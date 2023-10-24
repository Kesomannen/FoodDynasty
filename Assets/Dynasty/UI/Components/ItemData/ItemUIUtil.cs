using System.Collections.Generic;
using Google.MaterialDesign.Icons;
using UnityEngine;

namespace Dynasty.UI.Components {

public static class ItemUIUtil {
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
        { ItemType.Food, "e57a" },
    };
    
    static readonly Dictionary<ItemTier, Color> _colorLookup = new() {
        { ItemTier.Rusty, new Color(0.62f, 0.3f, 0.25f) },
        { ItemTier.Metallic, new Color(0.6f, 0.6f, 0.6f) }
    };

    public static void SetFromItemType(MaterialIcon icon, ItemType type) {
        icon.iconUnicode = GetIconUnicode(type);
    }

    public static string GetIconUnicode(ItemType type) {
        return _iconUnicodeLookup.TryGetValue(type, out var value) ? value : FallbackIconUnicode;
    }
    
    public static Color GetColor(ItemTier tier) {
        return _colorLookup.TryGetValue(tier, out var color) ? color : _fallbackColor;
    }
}

}