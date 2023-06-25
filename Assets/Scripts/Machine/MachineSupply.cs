using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class MachineSupply : SupplyBase, IInfoProvider, IStatusProvider {
    [SerializeField] int _requiredSupplyPerUse = 1;
    [SerializeField] Optional<InventoryItemData> _refillItem;
    [Foldout("Override")]
    [SerializeField] string _refillItemName;
    [Space]
    [SerializeField] CheckEvent<bool> _condition;
    [SerializeField] GenericEvent _onUsed;

    int _currentSupply;

    protected override string ItemNameOverride => _refillItemName;

    public event Action<IStatusProvider> OnStatusChanged;
    public override event Action<SupplyBase> OnChanged;

    public override int CurrentSupply {
        get => _currentSupply;
        set {
            if (_currentSupply == value) return;
            _currentSupply = value;
            
            OnStatusChanged?.Invoke(this);
            OnChanged?.Invoke(this);
        }
    }

    public override bool IsRefillable => _refillItem.Enabled;
    public override InventoryItemData RefillItem => _refillItem.Value;

    bool HasSupply() => CurrentSupply >= _requiredSupplyPerUse;
    void OnUsed() => CurrentSupply -= _requiredSupplyPerUse;
    
    void OnEnable() {
        _condition.AddCondition(HasSupply);
        _onUsed.OnRaisedGeneric += OnUsed;
    }
    
    void OnDisable() {
        _condition.RemoveCondition(HasSupply);
        _onUsed.OnRaisedGeneric -= OnUsed;
    }

    public IEnumerable<(string Name, string Value)> GetInfo() {
        if (_refillItem.Enabled) {
            yield return ("Refill", RefillItemName);
        }
    }

    public IEnumerable<(string Name, string Value)> GetStatus() {
        yield return (RefillItemName, _currentSupply.ToString());
    }
}