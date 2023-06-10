using System;
using UnityEngine;

public class Tooltip<T> : MonoBehaviour {
    [SerializeField] Vector2 _mouseOffset;
    [SerializeField] Container<T> _tooltip;
    [SerializeField] GameEvent<TooltipParams> _showTooltipEvent;
    [SerializeField] GenericGameEvent _hideTooltipEvent;

    RectTransform _tooltipRect;
    Transform _currentLockPoint;

    bool _currentLockX;
    bool _currentLockY;

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

    public void Show(TooltipParams tooltipParams) {
        Show((T) tooltipParams.Content, tooltipParams.LockAxis, tooltipParams.LockPoint);
    }
    
    public void Show(T content, TooltipLockAxis lockAxis = TooltipLockAxis.None, Transform lockPoint = null) {
        _currentLockX = lockAxis.HasFlag(TooltipLockAxis.X);
        _currentLockY = lockAxis.HasFlag(TooltipLockAxis.Y);
        _currentLockPoint = lockPoint;
        
        _tooltip.SetContent(content);
        _tooltip.gameObject.SetActive(true);
    }
    
    public void Hide() {
        _tooltip.gameObject.SetActive(false);
    }
    
    void Update() {
        if (!_tooltip.isActiveAndEnabled) return;
        UpdatePosition();
    }

    void UpdatePosition() {
        var mousePos = MouseHelpers.MouseScreenPosition;
        var pivot = Vector2.one * 0.5f;

        var screenPos = MouseHelpers.MouseScreenPosition + _mouseOffset;
        var screenSize = CanvasHelpers.CanvasToScreen(_tooltipRect.sizeDelta);

        pivot.x = screenPos.x / Screen.width;
        pivot.y = screenPos.y - screenSize.y < 0 ? 0 : 1;
        
        var lockPos = _currentLockPoint.position;
        if (_currentLockX) {
            mousePos.x = lockPos.x;
            pivot.x = lockPos.x < mousePos.x ? 0 : 1;
        }
        
        if (_currentLockY) {
            mousePos.y = lockPos.y;
            pivot.y = lockPos.y < mousePos.y ? 0 : 1;
            pivot.x = 0.5f;
        }

        if (_currentLockX) {
            pivot.y = 0.5f;
        }

        _tooltipRect.pivot = pivot;
        _tooltipRect.position = mousePos;
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