using System;
using Dynasty.Library.Classes;
using Dynasty.Library.Events;
using UnityEngine;
using UnityEngine.Events;

namespace Dynasty.UI.Controllers {

public class TabController : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] int _startTabIndex;
    [SerializeField] TabData[] _tabs;
    
    [Header("Animation")]
    [SerializeField] float _animationDuration;
    [SerializeField] Vector2 _hiddenTabSizeMultiplier;
    [SerializeField] LeanTweenType _animationEaseType;
    
    [Header("Events")]
    [SerializeField] IntEvent _onTabChange;
    [SerializeField] UnityEvent _onTabDoubleClick;
    
    [Serializable]
    class IntEvent : UnityEvent<int> {}

    Vector2 _hiddenTabSize;
    Vector2 _defaultTabSize;

    int _currentTabIndex;
    int[] _tweenIds = Array.Empty<int>();

    void OnEnable() {
        foreach (var tabData in _tabs) {
            tabData.Subscribe(this);
        }
    }

    void OnDisable() {
        foreach (var tabData in _tabs) {
            tabData.Unsubscribe();
        }
    }

    void Start() {
        _defaultTabSize = _tabs[0].Indicator.sizeDelta;
        _hiddenTabSize = _defaultTabSize * _hiddenTabSizeMultiplier;
        
        ChangeTab(_startTabIndex, true);
    }

    public void ChangeTab(int index) {
        ChangeTab(index, false);
    }

    void ChangeTab(int index, bool force) {
        if (!force && index == _currentTabIndex) {
            _onTabDoubleClick.Invoke();
            ChangeTab(-1);
            return;
        }
        
        var indexOutOfRange = index < 0 || index >= _tabs.Length;
        
        for (var i = 0; i < _tabs.Length; i++) {
            var isSelected = indexOutOfRange ? i == _currentTabIndex : i == index;
            _tabs[i].Content.SetActive(isSelected);
        }

        _currentTabIndex = index;
        
        foreach (var tweenId in _tweenIds) {
            LeanTween.cancel(tweenId);
        }

        _tweenIds = new int[_tabs.Length];
        for (var i = 0; i < _tabs.Length; i++) {
            var tab = _tabs[i];
            var size = i == index ? _defaultTabSize : _hiddenTabSize;

            _tweenIds[i] = LeanTween
                .size(tab.Indicator, size, _animationDuration)
                .setEase(_animationEaseType)
                .uniqueId;
        }

        if (indexOutOfRange) return;
        _onTabChange.Invoke(index);
    }

    [Serializable]
    class TabData {
        [SerializeField] RectTransform _indicator;
        [SerializeField] GameObject _content;
        [SerializeField] Optional<GenericGameEvent> _openEvent;
        
        public RectTransform Indicator => _indicator;
        public GameObject Content => _content;

        TabController _controller;
        
        public void Subscribe(TabController controller) {
            if (!_openEvent.Enabled) return;
            _openEvent.Value.AddListener(OnRaised);
            _controller = controller;
        }
        
        public void Unsubscribe() {
            if (!_openEvent.Enabled) return;
            _openEvent.Value.RemoveListener(OnRaised);
            _controller = null;
        }

        void OnRaised() {
            if (_controller == null) return;
            var index = Array.IndexOf(_controller._tabs, this);
            _controller.ChangeTab(index);
        }
    }
}

}