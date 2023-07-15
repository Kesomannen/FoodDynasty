using UnityEngine;

namespace Dynasty.UI.Controllers {

public class CloseableWindowController : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] bool _startOpen;
    [SerializeField] RectTransform _window;
    [SerializeField] Canvas _windowCanvas;
    [SerializeField] RectTransform _openPosition;
    [SerializeField] RectTransform _closePosition;
    
    [Header("Animation")]
    [SerializeField] float _animationDuration;
    [SerializeField] LeanTweenType _animationEaseType;

    bool _currentState;
    int _tweenId;

    void Awake() {
        _currentState = _startOpen;
        SetState(_currentState, tween: false, force: true);
    }
    
    public void Open() {
        SetState(true);
    }
    
    public void Close() {
        SetState(false);
    }
    
    public void Toggle() {
        SetState(!_currentState);
    }

    void SetState(bool state, bool tween = true, bool force = false) {
        if (!force && _currentState == state) return;
        
        _currentState = state;
        var targetPosition = state ? _openPosition : _closePosition;
        
        LeanTween.cancel(_tweenId);
        if (tween) {
            _windowCanvas.enabled = true;

            _tweenId = LeanTween
                .move(_window, targetPosition.anchoredPosition, _animationDuration)
                .setEase(_animationEaseType)
                .setOnComplete(() => {
                    if (!state) _windowCanvas.enabled = false;
                }).uniqueId;
        } else {
            _window.anchoredPosition = targetPosition.anchoredPosition;
            _windowCanvas.enabled = state;
        }
    }
}

}