using System;
using UnityEngine;

#pragma warning disable 0067

public class SupplyBase : MonoBehaviour {
    public virtual int CurrentSupply { get; set; }

    public virtual bool IsRefillable => false;
    public virtual InventoryItemData RefillItem => null;
    public string RefillItemName => IsRefillable ? RefillItem.Name : ItemNameOverride;
    
    protected virtual string ItemNameOverride => "Unknown item";

    public virtual event Action<SupplyBase> OnChanged;
}