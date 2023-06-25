using UnityEngine;

public static class TextStyles {
    static readonly Style _inventoryItem = new(new Color(1f, 1f, 0.8f));

    public static string InventoryItem(string text) {
        return _inventoryItem.Apply(text);
    }

    readonly struct Style {
        readonly Color _color;
        
        public Style(Color color) {
            _color = color;
        }

        string OpenTag => $"<color=#{ColorUtility.ToHtmlStringRGB(_color)}>";
        const string CloseTag = "</color>";
        
        public string Apply(string text) {
            return $"{OpenTag}{text}{CloseTag}";
        }
    }
}