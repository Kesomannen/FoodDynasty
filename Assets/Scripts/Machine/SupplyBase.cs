using System;
using UnityEngine;

#pragma warning disable 0067

public class SupplyBase : MonoBehaviour {
    public virtual int MaxSupply { get; set; }
    public virtual int CurrentSupply { get; set; }
    
    public virtual IOptional<InventoryItemData> RefillItem { get; }

    public virtual event Action<SupplyBase> OnChanged;
}