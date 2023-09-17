using Dynasty.Core.Tooltip;
using UnityEngine;

namespace Dynasty.UI.Tooltip {

public static class TooltipUtil {
    public static void PositionAsTooltip(this RectTransform tooltip, Vector2 position, bool useLocalPosition = false) {
        tooltip.pivot = new Vector2(position.x / Screen.width, 1);
        if (useLocalPosition) {
            tooltip.localPosition = position;
        } else {
            tooltip.position = position;
        }
    }
    
    public static void PositionAsTooltip(this RectTransform tooltip, Vector2 position, TooltipLockAxis lockAxis, Transform lockPoint, bool useLocalPosition = false) {
        if (lockAxis == TooltipLockAxis.None) {
            tooltip.PositionAsTooltip(position);
            return;
        }
        
        var pivot = new Vector2(position.x / Screen.width, 1);
        var lockX = lockAxis.HasFlag(TooltipLockAxis.X);
        var lockY = lockAxis.HasFlag(TooltipLockAxis.Y);
        
        var lockPos = lockPoint.position;
        if (lockX) {
            position.x = lockPos.x;
            pivot.x = lockPos.x < position.x ? 0 : 1;
        }
            
        if (lockY) {
            position.y = lockPos.y;
            pivot.y = lockPos.y < position.y ? 0 : 1;
        }

        tooltip.pivot = pivot;
        if (useLocalPosition) {
            tooltip.localPosition = position;
        } else {
            tooltip.position = position;
        }
    }
}

}