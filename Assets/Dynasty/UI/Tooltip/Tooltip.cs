using System.Collections;
using Dynasty.Core.Tooltip;
using Dynasty.Library.Events;
using Dynasty.Library.Helpers;
using Dynasty.UI.Components;
using UnityEngine;
using UnityEngine.Events;

namespace Dynasty.UI.Tooltip {

public class Tooltip<T> : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] float _delay = 0.1f;
    [SerializeField] Vector2 _mouseOffset;
    [SerializeField] Container<T> _tooltip;

    [Header("Events")]
    [SerializeField] GameEvent<TooltipParams> _showTooltipEvent;
    [SerializeField] GenericGameEvent _hideTooltipEvent;
    [SerializeField] UnityEvent<T> _onTooltipShown;

    RectTransform _tooltipRect;
    TooltipLockAxis _currentLockAxis;
    Transform _currentLockPoint;
    Coroutine _delayCoroutine;

    void Awake() {
        _tooltipRect = _tooltip.GetComponent<RectTransform>();
        _tooltip.gameObject.SetActive(false);
    }

    void OnEnable() {
        _showTooltipEvent.AddListener(Show);
        _hideTooltipEvent.AddListener(Hide);
    }
    
    void OnDisable() {
        _showTooltipEvent.RemoveListener(Show);
        _hideTooltipEvent.RemoveListener(Hide);
    }
    
    void Update() {
        if (!_tooltip.isActiveAndEnabled) return;
        UpdatePosition();
    }

    void Show(TooltipParams tooltipParams) {
        Show((T) tooltipParams.Content, tooltipParams.LockAxis, tooltipParams.LockPoint);
    }

    void Show(T content, TooltipLockAxis lockAxis = TooltipLockAxis.None, Transform lockPoint = null) {
        CancelDelay();
        _delayCoroutine = StartCoroutine(DelayRoutine());
        
        IEnumerator DelayRoutine() {
            yield return CoroutineHelpers.Wait(_delay);
            
            _tooltip.SetContent(content);
        
            _currentLockAxis = lockAxis;
            _currentLockPoint = lockPoint;
        
            _tooltip.gameObject.SetActive(true);
            _onTooltipShown.Invoke(content);
            
            UpdatePosition();
        }
    }

    void Hide() {
        CancelDelay();
        _tooltip.gameObject.SetActive(false);
    }

    void CancelDelay() {
        if (_delayCoroutine == null) return;
        StopCoroutine(_delayCoroutine);
        _delayCoroutine = null;
    }

    void UpdatePosition() {
        var pos = MouseHelpers.ScreenPosition + _mouseOffset;
        _tooltipRect.PositionAsTooltip(pos, _currentLockAxis, _currentLockPoint);
    }
}

}