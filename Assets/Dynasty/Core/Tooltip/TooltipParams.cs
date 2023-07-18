using UnityEngine;

namespace Dynasty.Core.Tooltip {

/// <summary>
/// Passed to a <see cref="Tooltip"/> or event to specify how it should be displayed.
/// </summary>
public struct TooltipParams {
    public object Content;
    public TooltipLockAxis LockAxis;
    public Transform LockPoint;
}

}