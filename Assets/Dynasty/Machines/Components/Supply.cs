using System;
using System.Collections.Generic;
using Dynasty.Library.Data;
using Dynasty.Core.Inventory;
using Dynasty.Library;
using Dynasty.Library.Events;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dynasty.Machines {

[RequireComponent(typeof(Outline))]
public class Supply : MonoBehaviour, IStatusProvider, IInfoProvider, IAdditionalSaveData<int>, IMachineComponent, IOnDeletedHandler {
    [SerializeField] int _requiredSupplyPerUse = 1;
    [SerializeField] ItemData _refillItem;
    [SerializeField] string _refillItemName;
    [SerializeField] CheckEvent<bool> _condition;
    [SerializeField] Color _emptyOutlineColor = Color.red;
    [FormerlySerializedAs("_onUsed")]
    [SerializeField] GenericEvent _useEvent;

    int _currentSupply;
    Color _defaultOutlineColor;
    Outline _outline;
    
    public const int MaxSupply = int.MaxValue;

    public ItemData RefillItem {
        get => _refillItem;
        set => _refillItem = value;
    }

    public CheckEvent<bool> Condition {
        get => _condition;
        set => _condition = value;
    }
    
    public GenericEvent ConsumeEvent {
        get => _useEvent;
        set => _useEvent = value;
    }
    
    public int CurrentSupply {
        get => _currentSupply;
        set {
            if (_currentSupply == value) return;
            _currentSupply = value;
            
            UpdateOutline();
            
            OnStatusChanged?.Invoke(this);
            OnChanged?.Invoke(this);
        }
    }
    
    public bool IsRefillable => RefillItem != null;
    
    public string RefillItemName {
        get => IsRefillable ? RefillItem.Name : _refillItemName;
        set => _refillItemName = value;
    }

    public event Action<IStatusProvider> OnStatusChanged;
    public event Action<Supply> OnChanged;

    void Awake() {
        _outline = GetComponent<Outline>();
        _defaultOutlineColor = _outline.OutlineColor;
        
        UpdateOutline();
    }

    void OnEnable() {
        _condition.AddCriterion(HasSupply);
        _useEvent.OnRaisedGeneric += OnUsed;
    }
    
    void OnDisable() {
        _condition.RemoveCriterion(HasSupply);
        _useEvent.OnRaisedGeneric -= OnUsed;
    }

    public void Empty(InventoryAsset inventory) {
        if (IsRefillable) {
            inventory.Add(RefillItem, CurrentSupply);
        }
        CurrentSupply = 0;
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
    
    public void OnDeleted(InventoryAsset toInventory) {
        Empty(toInventory);
    }
    
    void UpdateOutline() {
        var hasSupply = HasSupply();
        _outline.OutlineColor = hasSupply ? _defaultOutlineColor : _emptyOutlineColor;
        _outline.enabled = !hasSupply;
    }

    public Component Component => this;
}

}