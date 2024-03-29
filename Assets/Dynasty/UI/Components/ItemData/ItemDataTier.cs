﻿using Dynasty;
using Dynasty.Library;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dynasty.UI.Components {

public class ItemDataTier : UIComponent<ItemData> {
    [SerializeField] Optional<TMP_Text> _displayText;
    [SerializeField] Graphic[] _graphicsToColor;

    public override void SetContent(ItemData content) {
        var tier = content.Tier;
        
        if (_displayText.Enabled) {
            _displayText.Value.text = tier.ToString();
        }
        
        foreach (var graphic in _graphicsToColor) {
            graphic.color = ItemUIUtil.GetColor(tier);
        }
    }
}

}