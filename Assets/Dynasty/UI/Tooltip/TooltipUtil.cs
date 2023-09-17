using Dynasty.Core.Tooltip;
using UnityEngine;

namespace Dynasty.UI.Tooltip {

public static class TooltipUtil {
    public static void PositionAsTooltip(this RectTransform tooltip, Vector2 position) {
        tooltip.pivot = new Vector2(position.x / Screen.width, 1);
        tooltip.position = position;
    }
    
    public static void PositionAsTooltip(this RectTransform tooltip, Vector2 position, TooltipLockAxis lockAxis, Transform lockPoint) {
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
        tooltip.position = position;
    }
}

}