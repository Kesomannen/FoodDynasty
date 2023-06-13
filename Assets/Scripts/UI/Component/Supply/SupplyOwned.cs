using UnityEngine;

public class SupplyOwned : UIComponent<SupplyBase> {
    [SerializeField] UIComponent<InventoryItemData> _dataComponent;
    [SerializeField] GameObject[] _hideIfNotRefillable;

    public override void SetContent(SupplyBase content) {
        if (content.IsRefillable) {
            _dataComponent.SetContent(content.RefillItem);
            return;
        }

        foreach (var obj in _hideIfNotRefillable) {
            obj.SetActive(false);
        }
    }
}