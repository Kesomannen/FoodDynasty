using System;
using UnityEngine;
using UnityEngine.Events;

public class Tooltip<T> : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] Vector2 _mouseOffset;
    [SerializeField] Container<T> _tooltip;

    [Header("Events")]
    [SerializeField] GameEvent<TooltipParams> _showTooltipEvent;
    [SerializeField] GenericGameEvent _hideTooltipEvent;
    [SerializeField] UnityEvent<T> _onTooltipShown;

    RectTransform _tooltipRect;
    TooltipLockAxis _currentLockAxis;
    Transform _currentLockPoint;

    void Awake() {
        _tooltipRect = _tooltip.GetComponent<RectTransform>();
        _tooltip.gameObject.SetActive(false);
    }

    void OnEnable() {
        _showTooltipEvent.OnRaised += Show;
        _hideTooltipEvent.OnRaisedGeneric += Hide;
    }
    
    void OnDisable() {
        _showTooltipEvent.OnRaised -= Show;
        _hideTooltipEvent.OnRaisedGeneric -= Hide;
    }

    protected virtual bool OnTooltipShown(T content) {
        _tooltip.SetContent(content);
        return true;
    }

    public void Show(TooltipParams tooltipParams) {
        Show((T) tooltipParams.Content, tooltipParams.LockAxis, tooltipParams.LockPoint);
    }
    
    public void Show(T content, TooltipLockAxis lockAxis = TooltipLockAxis.None, Transform lockPoint = null) {
        if (!OnTooltipShown(content)) return;
        
        _currentLockAxis = lockAxis;
        _currentLockPoint = lockPoint;
        
        _tooltip.gameObject.SetActive(true);
        _onTooltipShown.Invoke(content);
    }
    
    public void Hide() {
        _tooltip.gameObject.SetActive(false);
    }
    
    void Update() {
        if (!_tooltip.isActiveAndEnabled) return;
        UpdatePosition();
    }

    void UpdatePosition() {
        var pos = MouseHelpers.ScreenPosition + _mouseOffset;
        _tooltipRect.PositionAsTooltip(pos, _currentLockAxis, _currentLockPoint);
    }
}

public struct TooltipParams {
    public object Content;
    public TooltipLockAxis LockAxis;
    public Transform LockPoint;
}

[Flags]
public enum TooltipLockAxis {
    None = 0,
    X = 1,
    Y = 2
}