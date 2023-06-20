using System;
using TMPro;
using UnityEngine;

public class InventoryItemDataOwned : UIComponent<InventoryItemData> {
    [SerializeField] TMP_Text _text;
    [SerializeField] string _format = "{0}";
    [SerializeField] InventoryAsset _inventory;
    
    InventoryItemData _content;
    
    public override void SetContent(InventoryItemData content) {
        _content = content;
        _text.text = string.Format(_format, _inventory.GetCount(content));
    }

    void OnEnable() {
        _inventory.OnItemChanged += OnItemChanged;
    }
    
    void OnDisable() {
        _inventory.OnItemChanged -= OnItemChanged;
    }

    void OnItemChanged(InventoryItem item) {
        if (item.Data != _content) return;
        _text.text = string.Format(_format, item.Count);
    }
}