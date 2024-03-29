﻿using Dynasty.UI;
using Dynasty.Library;
using UnityEngine;

namespace Dynasty.UI.Tooltip {

public static class TooltipUtil {
    public static void PositionAsTooltip(this RectTransform tooltip, Vector2 position, TooltipLockAxis lockAxis = TooltipLockAxis.None, Transform lockPoint = null, bool useLocalPosition = false) {
        var pivot = new Vector2(position.x / Screen.width, 1);
        var lockX = lockAxis.HasFlag(TooltipLockAxis.X);
        var lockY = lockAxis.HasFlag(TooltipLockAxis.Y);

        if (lockPoint != null) {
            var lockPos = lockPoint.position;
            if (lockX) {
                position.x = lockPos.x;
                pivot.x = lockPos.x < position.x ? 0 : 1;
                pivot.y = 0.5f;
            }
            
            if (lockY) {
                position.y = lockPos.y;
                pivot.y = lockPos.y < position.y ? 0 : 1;
            }
        }

        var size = CanvasHelpers.ScreenToCanvas(tooltip.rect.size);

        if (position.x + size.x * (1 - pivot.x) > Screen.width) {
            position.x = Screen.width - size.x;
            pivot.x = 1;
        } else if (position.x - size.x * pivot.x < 0) {
            position.x = size.x;
            pivot.x = 0;
        } 
        
        if (position.y + size.y * (1 - pivot.y) > Screen.height) {
            position.y = Screen.height - size.y;
            pivot.y = 0;
        } else if (position.y - size.y * pivot.y < 0) {
            position.y = size.y;
            pivot.y = 1;
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