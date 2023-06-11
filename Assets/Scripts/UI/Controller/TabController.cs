using System;
using UnityEngine;

public class TabController : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] int _startTabIndex;
    [SerializeField] TabData[] _tabs;
    
    [Header("Animation")]
    [SerializeField] float _animationDuration;
    [SerializeField] Vector2 _hiddenTabSizeMultiplier;
    [SerializeField] LeanTweenType _animationEaseType;

    Vector2 _hiddenTabSize;
    Vector2 _defaultTabSize;

    int _currentTabIndex;
    int[] _tweenIds = Array.Empty<int>();
    
    void Start() {
        _defaultTabSize = _tabs[0].Indicator.sizeDelta;
        _hiddenTabSize = _defaultTabSize * _hiddenTabSizeMultiplier;
        
        ChangeTab(_startTabIndex, true);
    }
    
    public void ChangeTab(int index) {
        ChangeTab(index, false);
    }

    void ChangeTab(int index, bool force) {
        if (!force && index == _currentTabIndex) return;
        
        for (var i = 0; i < _tabs.Length; i++) {
            _tabs[i].Content.SetActive(i == index);
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
    }

    [Serializable]
    struct TabData {
        public RectTransform Indicator;
        public GameObject Content;
    }
}