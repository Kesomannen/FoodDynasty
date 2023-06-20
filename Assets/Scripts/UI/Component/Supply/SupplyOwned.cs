using UnityEngine;

public class SupplyOwned : UIComponent<SupplyBase> {
    [SerializeField] UIComponent<InventoryItemData> _dataComponent;
    [SerializeField] GameObject[] _hideIfNotRefillable;

    public override void SetContent(SupplyBase content) {
        var refillable = content.RefillItem.Enabled;

        foreach (var obj in _hideIfNotRefillable) {
            obj.SetActive(refillable);
        }

        if (!refillable) return;
        _dataComponent.SetContent(content.RefillItem.Value);
    }
}