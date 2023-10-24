using Dynasty.Core.Inventory;
using Dynasty.Machines;
using Dynasty.UI.Controllers;
using UnityEngine;

namespace Dynasty.UI.Components {

public class SupplyInput : UIComponent<Supply> {
    [SerializeField] NumberInputController _input;
    [SerializeField] InventoryAsset _inventory;
    [SerializeField] NumberInputController.ModifyMode _modifyMode;

    Supply _content;

    public override void SetContent(Supply content) {
        _content = content;

        if (!_content.IsRefillable) return;

        _input.Initialize(
            mode: _modifyMode, 
            startValue: GetMaxValue(),
            maxValue: GetMaxValue
        );
            
        return;

        float GetMaxValue() => _inventory.GetCount(content.RefillItem);
    }
    
    public void AddCurrent() {
        Add(_input.Value);
    }

    void Add(float value) {
        var intValue = (int) value;
        if (intValue == 0) return; 
        if (!_inventory.Remove(_content.RefillItem, intValue)) return;
        
        _content.CurrentSupply += intValue;
    }
    
    void Subtract(float value) {
        var intValue = (int) value;
        if (intValue == 0) return;
        if (intValue > _content.CurrentSupply) intValue = _content.CurrentSupply;
        
        _content.CurrentSupply -= intValue;
        _inventory.Add(_content.RefillItem, intValue);
    }

    public void Empty() {
        _content.Empty(_inventory);
    }
}

}