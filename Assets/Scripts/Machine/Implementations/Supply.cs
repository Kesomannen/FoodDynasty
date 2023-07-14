using System;
using System.Collections.Generic;
using Dynasty.Library.Events;
using UnityEngine;
using UnityEngine.Serialization;

public class Supply : MonoBehaviour, IStatusProvider, IInfoProvider, IAdditionalSaveData<int> {
    [SerializeField] int _requiredSupplyPerUse = 1;
    [SerializeField] ItemData _refillItem;
    [SerializeField] string _refillItemName;
    [SerializeField] CheckEvent<bool> _condition;
    [FormerlySerializedAs("_onUsed")]
    [SerializeField] GenericEvent _useEvent;

    int _currentSupply;

    public ItemData RefillItem {
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
    
    public int CurrentSupply {
        get => _currentSupply;
        set {
            if (_currentSupply == value) return;
            _currentSupply = value;
            
            OnStatusChanged?.Invoke(this);
            OnChanged?.Invoke(this);
        }
    }
    
    public bool IsRefillable => RefillItem != null;
    public string RefillItemName => IsRefillable ? RefillItem.Name : _refillItemName;

    public event Action<IStatusProvider> OnStatusChanged;
    public event Action<Supply> OnChanged;

    void OnEnable() {
        _condition.AddCondition(HasSupply);
        _useEvent.OnRaisedGeneric += OnUsed;
    }
    
    void OnDisable() {
        _condition.RemoveCondition(HasSupply);
        _useEvent.OnRaisedGeneric -= OnUsed;
    }
    
    public IEnumerable<EntityInfo> GetInfo() {
        if (IsRefillable) {
            yield return new EntityInfo("Refill", RefillItemName);
        }
    }

    public IEnumerable<EntityInfo> GetStatus() {
        yield return new EntityInfo(RefillItemName, CurrentSupply.ToString());
    }
    
    bool HasSupply() => CurrentSupply >= _requiredSupplyPerUse;
    void OnUsed() => CurrentSupply -= _requiredSupplyPerUse;
    
    public void OnAfterLoad(int data) {
        CurrentSupply = data;
    }

    public int GetSaveData() {
        return CurrentSupply;
    }
}