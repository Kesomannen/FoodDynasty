using System;
using System.Collections.Generic;
using UnityEngine;

public class Supply : SupplyBase, IInfoProvider, IStatusProvider {
    [SerializeField] bool _refillable = true;
    [SerializeField] InventoryItemData _refillItem;
    [SerializeField] Optional<int> _maxSupply;
    [Space]
    [SerializeField] CheckEvent<bool> _condition;
    [SerializeField] GenericEvent _onUsed;

    int _currentSupply;

    public event Action<IStatusProvider> OnStatusChanged;
    public override event Action<SupplyBase> OnChanged;
    
    public override int MaxSupply {
        get => _maxSupply.Enabled ? _maxSupply.Value : int.MaxValue;
        set {
            if (_maxSupply.Enabled && _maxSupply.Value == value) return;
            _maxSupply = new Optional<int>(value);
            OnStatusChanged?.Invoke(this);
            OnChanged?.Invoke(this);
        }
    }

    public override int CurrentSupply {
        get => _currentSupply;
        set {
            var clamped = Mathf.Clamp(value, 0, MaxSupply);
            if (_currentSupply == clamped) return;
            _currentSupply = clamped;
            OnStatusChanged?.Invoke(this);
            OnChanged?.Invoke(this);
        }
    }

    public override bool IsRefillable => _refillable;
    public override InventoryItemData RefillItem => _refillItem;
    
    bool HasSupply() => CurrentSupply > 0;
    void OnUsed() => CurrentSupply--;
    
    void OnEnable() {
        _condition.AddCondition(HasSupply);
        _onUsed.OnRaisedGeneric += OnUsed;
    }
    
    void OnDisable() {
        _condition.RemoveCondition(HasSupply);
        _onUsed.OnRaisedGeneric -= OnUsed;
    }

    public IEnumerable<(string Name, string Value)> GetInfo() {
        if (_maxSupply.Enabled) {
            yield return ("Max Supply", _maxSupply.Value.ToString());
        }
    }

    public IEnumerable<(string Name, string Value)> GetStatus() {
        if (_maxSupply.Enabled) {
            yield return (_refillItem.Name, $"{_currentSupply}/{_maxSupply.Value}");
        } else {
            yield return (_refillItem.Name, _currentSupply.ToString());
        }
    }
}