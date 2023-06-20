using System;
using UnityEngine;

public class SupplyInput : UIComponent<SupplyBase> {
    [SerializeField] NumberInputController _input;
    [SerializeField] InventoryAsset _inventory;
    [SerializeField] NumberInputController.ModifyMode _modifyMode;

    SupplyBase _content;

    public override void SetContent(SupplyBase content) {
        _content = content;
        _input.Initialize(mode: _modifyMode, maxValue: () => _inventory.GetCount(content.RefillItem.Value));
    }
    
    public void AddCurrent() {
        Add(_input.Value);
    }
    
    public void SubtractCurrent() {
        Subtract(_input.Value);
    }

    void Add(float value) {
        if (_content == null || !_content.RefillItem.Enabled) return;

        var intValue = (int) value;
        if (intValue == 0) return; 
        if (!_inventory.Remove(_content.RefillItem.Value, intValue)) return;
        
        _content.CurrentSupply += intValue;
    }
    
    void Subtract(float value) {
        if (_content == null || !_content.RefillItem.Enabled) return;
        
        var intValue = (int) value;
        if (intValue == 0) return;
        if (intValue > _content.CurrentSupply) intValue = _content.CurrentSupply;
        
        _content.CurrentSupply -= intValue;
        _inventory.Add(_content.RefillItem.Value, intValue);
    }

    public void Empty() {
        if (_content == null || !_content.RefillItem.Enabled) return;
        Subtract(_content.CurrentSupply);
    }
}