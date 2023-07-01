using System;
using TMPro;
using UnityEngine;

public class ItemDataOwned : UIComponent<ItemData> {
    [SerializeField] TMP_Text _text;
    [SerializeField] string _format = "{0}";
    [SerializeField] InventoryAsset _inventory;
    
    ItemData _content;
    
    public override void SetContent(ItemData content) {
        _content = content;
        _text.text = string.Format(_format, _inventory.GetCount(content));
    }

    void OnEnable() {
        _inventory.OnItemChanged += OnItemChanged;
    }
    
    void OnDisable() {
        _inventory.OnItemChanged -= OnItemChanged;
    }

    void OnItemChanged(Item item) {
        if (item.Data != _content) return;
        _text.text = string.Format(_format, item.Count);
    }
}