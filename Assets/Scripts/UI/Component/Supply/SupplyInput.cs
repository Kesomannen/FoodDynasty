using UnityEngine;

public class SupplyInput : UIComponent<SupplyBase> {
    [SerializeField] NumberInputController _input;
    [SerializeField] InventoryAsset _inventory;
    [SerializeField] NumberInputController.ModifyMode _modifyMode;

    SupplyBase _content;

    public override void SetContent(SupplyBase content) {
        if (!content.IsRefillable) {
            Debug.LogWarning("Content Supply for SupplyInput is not refillable.", content);
        }
        
        _content = content;
        _input.Initialize(mode: _modifyMode, maxValue: () => _inventory.GetCount(content.RefillItem));
    }
    
    public void AddCurrent() {
        Add(_input.Value);
    }
    
    public void SubtractCurrent() {
        Subtract(_input.Value);
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
        Subtract(_content.CurrentSupply);
    }
}