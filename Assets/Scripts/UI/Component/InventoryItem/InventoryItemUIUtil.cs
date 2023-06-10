using System.Collections.Generic;
using Google.MaterialDesign.Icons;
using UnityEngine;

public static class InventoryItemUIUtil {
    const string FallbackIconUnicode = "ef55";
    
    static readonly Dictionary<InventoryItemType, string> _iconUnicodeLookup = new() {
        { InventoryItemType.Cooker, "ef55" }
    };
    
    static readonly Dictionary<InventoryItemTier, Color> _colorLookup = new() {
        { InventoryItemTier.Rusty, new Color(0.45f, 0.26f, 0.22f) }
    };

    public static void SetFromItemType(this MaterialIcon icon, InventoryItemType type) {
        icon.iconUnicode = GetIconUnicode(type);
    }

    public static string GetIconUnicode(InventoryItemType type) {
        return _iconUnicodeLookup.TryGetValue(type, out var value) ? value : FallbackIconUnicode;
    }
    
    public static Color GetColor(InventoryItemTier tier) {
        return _colorLookup.TryGetValue(tier, out var color) ? color : Color.white;
    }
}