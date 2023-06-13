using System;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public struct TooltipData<T> {
    [SerializeField] TooltipLockAxis _tooltipLockAxis;
    [HideIf("_tooltipLockAxis", TooltipLockAxis.None)]
    [SerializeField] Transform _tooltipLockPoint;
    [SerializeField] GameEvent<TooltipParams> _showTooltipEvent;
    [SerializeField] GenericGameEvent _hideTooltipEvent;
    
    public TooltipParams GetParams(T content) {
        return new TooltipParams {
            LockPoint = _tooltipLockPoint,
            LockAxis = _tooltipLockAxis,
            Content = content
        };
    }
    
    public void Show(T content) {
        _showTooltipEvent.Raise(GetParams(content));
    }
    
    public void Hide() {
        _hideTooltipEvent.Raise();
    }

    public void Show(T content, bool show) {
        if (show) Show(content);
        else Hide();
    }
}