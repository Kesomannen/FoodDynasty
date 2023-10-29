using Dynasty;
using TMPro;
using UnityEngine;

namespace Dynasty.UI.Components {

public class ItemDataOwned : UpdatingUIComponent<ItemData> {
    [SerializeField] TMP_Text _text;
    [SerializeField] string _format = "{0}";
    [SerializeField] InventoryAsset _inventory;

    protected override void Subscribe(ItemData content) {
        _inventory.OnItemChanged += OnItemChanged;
        OnItemChanged(_inventory.GetOrAdd(content));
    }

    protected override void Unsubscribe(ItemData content) {
        _inventory.OnItemChanged -= OnItemChanged;
    }

    void OnItemChanged(Item item) {
        if (item.Data != Content) return;
        _text.text = string.Format(_format, item.Count);
    }
}

}