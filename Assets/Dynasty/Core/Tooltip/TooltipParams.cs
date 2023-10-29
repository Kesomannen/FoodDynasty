using UnityEngine;

namespace Dynasty.UI {

/// <summary>
/// Passed to a <see cref="UI"/> or event to specify how it should be displayed.
/// </summary>
public struct TooltipParams {
    public object Content;
    public TooltipLockAxis LockAxis;
    public Transform LockPoint;
}

}