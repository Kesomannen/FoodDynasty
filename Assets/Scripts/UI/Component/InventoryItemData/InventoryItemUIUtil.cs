using System.Collections.Generic;
using Google.MaterialDesign.Icons;
using UnityEngine;

public static class InventoryItemUIUtil {
    const string FallbackIconUnicode = "eb8b";
    static readonly Color _fallbackColor = Color.white;

    static readonly Dictionary<InventoryItemType, string> _iconUnicodeLookup = new() {
        { InventoryItemType.Cooker, "ef55" },
        { InventoryItemType.Conveyor, "ea50" },
        { InventoryItemType.Dispenser, "f181" },
        { InventoryItemType.Modifier, "e86b" },
        { InventoryItemType.Other, "eb8b" },
        { InventoryItemType.Seller, "e227"},
        { InventoryItemType.Topping, "f00c" },
        { InventoryItemType.BaseIngredient, "e57a" }
    };
    
    static readonly Dictionary<InventoryItemTier, Color> _colorLookup = new() {
        { InventoryItemTier.Rusty, new Color(0.45f, 0.25f, 0.2f) }
    };

    public static void SetFromItemType(this MaterialIcon icon, InventoryItemType type) {
        icon.iconUnicode = GetIconUnicode(type);
    }

    public static string GetIconUnicode(InventoryItemType type) {
        return _iconUnicodeLookup.TryGetValue(type, out var value) ? value : FallbackIconUnicode;
    }
    
    public static Color GetColor(InventoryItemTier tier) {
        return _colorLookup.TryGetValue(tier, out var color) ? color : _fallbackColor;
    }
}