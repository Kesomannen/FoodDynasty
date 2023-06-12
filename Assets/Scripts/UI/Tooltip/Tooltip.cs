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
        var position = MouseHelpers.MouseScreenPosition + _mouseOffset;
        var pivot = new Vector2(position.x / Screen.width, 1f);

        var lockPosition = !_currentLockX && !_currentLockY ? Vector3.zero : _currentLockPoint.position;
        if (_currentLockX) {
            position.x = lockPosition.x;
            pivot.x = lockPosition.x < position.x ? 0 : 1;
        }
        
        if (_currentLockY) {
            position.y = lockPosition.y;
            pivot.y = lockPosition.y < position.y ? 0 : 1;
            pivot.x = position.x / Screen.width;
        }

        if (_currentLockX) {
            pivot.y = position.y / Screen.height;
        }

        _tooltipRect.pivot = pivot;
        _tooltipRect.position = position;
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