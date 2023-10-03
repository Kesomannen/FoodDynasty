using Dynasty.Core.Inventory;
using Dynasty.Machines;
using UnityEngine;
using UnityEngine.Events;

namespace Dynasty.UI.Components {

public class SupplyOwned : UIComponent<Supply> {
    [SerializeField] UIComponent<ItemData> _dataComponent;
    [SerializeField] UnityEvent<bool> _onRefillableChanged;

    public override void SetContent(Supply content) {
        _onRefillableChanged.Invoke(content.IsRefillable);

        if (content.IsRefillable) {
            _dataComponent.SetContent(content.RefillItem);
        }
    }
}

}