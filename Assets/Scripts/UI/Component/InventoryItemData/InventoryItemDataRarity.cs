﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemDataRarity : UIComponent<InventoryItemData> {
    [SerializeField] Optional<TMP_Text> _displayText;
    [SerializeField] Graphic[] _graphicsToColor;

    public override void SetContent(InventoryItemData content) {
        var tier = content.Tier;
        
        if (_displayText.Enabled) {
            _displayText.Value.text = tier.ToString();
        }
        
        foreach (var graphic in _graphicsToColor) {
            graphic.color = InventoryItemUIUtil.GetColor(tier);
        }
    }
}