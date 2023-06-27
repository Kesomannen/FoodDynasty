using System;
using UnityEngine;

#pragma warning disable 0067

public class SupplyBase : MonoBehaviour, IOnDeletedHandler, IAdditionalSaveData {
    public virtual int CurrentSupply { get; set; }

    public virtual bool IsRefillable => false;
    public virtual ItemData RefillItem => null;
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
}