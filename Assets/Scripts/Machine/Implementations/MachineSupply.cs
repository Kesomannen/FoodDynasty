using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public class MachineSupply : SupplyBase, IInfoProvider, IStatusProvider{
    [SerializeField] int _requiredSupplyPerUse = 1;
    [SerializeField] ItemData _refillItem;
    [SerializeField] string _refillItemName;
    [SerializeField] CheckEvent<bool> _condition;
    [FormerlySerializedAs("_onUsed")]
    [SerializeField] GenericEvent _useEvent;

    int _currentSupply;

    public override int CurrentSupply {
        get => _currentSupply;
        set {
            if (_currentSupply == value) return;
            _currentSupply = value;
            
            OnStatusChanged?.Invoke(this);
            OnChanged?.Invoke(this);
        }
    }
    
    public override ItemData RefillItem {
        get => _refillItem;
        set => _refillItem = value;
    }

    public CheckEvent<bool> Condition {
        get => _condition;
        set => _condition = value;
    }
    
    public GenericEvent UseEvent {
        get => _useEvent;
        set => _useEvent = value;
    }

    protected override string ItemNameOverride => _refillItemName;

    public event Action<IStatusProvider> OnStatusChanged;
    public override event Action<SupplyBase> OnChanged;
    
    void OnEnable() {
        _condition.AddCondition(HasSupply);
        _useEvent.OnRaisedGeneric += OnUsed;
    }
    
    void OnDisable() {
        _condition.RemoveCondition(HasSupply);
        _useEvent.OnRaisedGeneric -= OnUsed;
    }
    
    bool HasSupply() => CurrentSupply >= _requiredSupplyPerUse;
    void OnUsed() => CurrentSupply -= _requiredSupplyPerUse;

    public IEnumerable<(string Name, string Value)> GetInfo() {
        if (IsRefillable) {
            yield return ("Refill", RefillItemName);
        }
    }

    public IEnumerable<(string Name, string Value)> GetStatus() {
        yield return (RefillItemName, _currentSupply.ToString());
    }
}