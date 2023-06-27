using System.Collections.Generic;
using Google.MaterialDesign.Icons;
using UnityEngine;

public static class InventoryItemUIUtil {
    const string FallbackIconUnicode = "eb8b";
    static readonly Color _fallbackColor = Color.white;

    static readonly Dictionary<ItemType, string> _iconUnicodeLookup = new() {
        { ItemType.Cooker, "ef55" },
        { ItemType.Conveyor, "ea50" },
        { ItemType.Dispenser, "f181" },
        { ItemType.Modifier, "e86b" },
        { ItemType.Other, "eb8b" },
        { ItemType.Seller, "e227"},
        { ItemType.Topping, "f00c" },
        { ItemType.BaseIngredient, "e57a" }
    };
    
    static readonly Dictionary<InventoryItemTier, Color> _colorLookup = new() {
        { InventoryItemTier.Rusty, new Color(0.62f, 0.3f, 0.25f) },
        { InventoryItemTier.Nuclear, new Color(0, 1, 0) }
    };

    public static void SetFromItemType(this MaterialIcon icon, ItemType type) {
        icon.iconUnicode = GetIconUnicode(type);
    }

    public static string GetIconUnicode(ItemType type) {
        return _iconUnicodeLookup.TryGetValue(type, out var value) ? value : FallbackIconUnicode;
    }
    
    public static Color GetColor(InventoryItemTier tier) {
        return _colorLookup.TryGetValue(tier, out var color) ? color : _fallbackColor;
    }
}