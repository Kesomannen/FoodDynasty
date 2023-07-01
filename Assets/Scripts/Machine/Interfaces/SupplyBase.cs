using System;
using UnityEngine;

#pragma warning disable 0067

public class SupplyBase : MonoBehaviour, IOnDeletedHandler, IAdditionalSaveData, IMachineComponent {
    public virtual int CurrentSupply { get; set; }

    public bool IsRefillable => RefillItem != null;

    public virtual ItemData RefillItem {
        get => null;
        set => throw new NotImplementedException();
    }

    public string RefillItemName => IsRefillable ? RefillItem.Name : ItemNameOverride;
    
    protected virtual string ItemNameOverride => "Unknown item";

    public virtual event Action<SupplyBase> OnChanged;

    public void OnDeleted(InventoryAsset toInventory) {
        if (!IsRefillable) return;
        toInventory.Add(RefillItem, CurrentSupply);
    }

    public void OnAfterLoad(object data) {
        CurrentSupply = (int) data;
    }

    public object GetSaveData() {
        return CurrentSupply;
    }
    
    public Component Component => this;
}