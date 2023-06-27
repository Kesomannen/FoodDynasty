using UnityEngine;

public class SupplyOwned : UIComponent<SupplyBase> {
    [SerializeField] UIComponent<ItemData> _dataComponent;
    [SerializeField] GameObject[] _hideIfNotRefillable;

    public override void SetContent(SupplyBase content) {
        foreach (var obj in _hideIfNotRefillable) {
            obj.SetActive(content.IsRefillable);
        }

        if (content.IsRefillable) {
            _dataComponent.SetContent(content.RefillItem);
        }
    }
}