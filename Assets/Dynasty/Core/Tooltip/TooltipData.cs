using System;
using Dynasty.Library.Events;
using UnityEngine;

namespace Dynasty.Core.Tooltip {

/// <summary>
/// Serializable abstraction for a tooltip display;
/// </summary>
[Serializable]
public class TooltipData<T> {
    [Tooltip("The axis(es) to lock the tooltip to.")]
    [SerializeField] TooltipLockAxis _tooltipLockAxis;
    
    [Tooltip("The point to lock the tooltip to, if any.")]
    [SerializeField] Transform _tooltipLockPoint;
    
    [Tooltip("Raised when the tooltip should be shown.")]
    [SerializeField] GameEvent<TooltipParams> _showTooltipEvent;
    
    [Tooltip("Raised when the tooltip should be hidden.")]
    [SerializeField] GenericGameEvent _hideTooltipEvent;
    
    /// <summary>
    /// Returns a <see cref="TooltipParams"/> instance for the given content.
    /// </summary>
    public TooltipParams GetParams(T content) {
        return new TooltipParams {
            LockPoint = _tooltipLockPoint,
            LockAxis = _tooltipLockAxis,
            Content = content
        };
    }
    
    /// <summary>
    /// Shows the tooltip with the given content.
    /// </summary>
    public void Show(T content) {
        _showTooltipEvent.Raise(GetParams(content));
    }
    
    /// <summary>
    /// Hides the tooltip.
    /// </summary>
    public void Hide() {
        _hideTooltipEvent.Raise();
    }

    /// <summary>
    /// Shows or hides the tooltip with the given content.
    /// </summary>
    public void Show(T content, bool show) {
        if (show) Show(content);
        else Hide();
    }
}

}