using Dynasty.Core.Inventory;
using Dynasty.Machine.Components;
using UnityEngine;

namespace Dynasty.UI.Components {

public class SupplyOwned : UIComponent<Supply> {
    [SerializeField] UIComponent<ItemData> _dataComponent;
    [SerializeField] GameObject[] _hideIfNotRefillable;

    public override void SetContent(Supply content) {
        foreach (var obj in _hideIfNotRefillable) {
            obj.SetActive(content.IsRefillable);
        }

        if (content.IsRefillable) {
            _dataComponent.SetContent(content.RefillItem);
        }
    }
}

}