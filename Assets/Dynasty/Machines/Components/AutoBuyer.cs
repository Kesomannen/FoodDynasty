using System;
using System.Collections;
using System.Collections.Generic;
using Dynasty.Core.Inventory;
using Dynasty.Library;
using Dynasty.Library.Data;
using Dynasty.Library.Helpers;
using Dynasty.Persistent.Mapping;
using UnityEngine;

namespace Dynasty.Machines {

public class AutoBuyer : MonoBehaviour,
    IMachineComponent,
    IBoostableProperty,
    IInfoProvider,
    IStatusProvider,
    IAdditionalSaveData<AutoBuyer.SaveData> 
{
    [SerializeField] InventoryAsset _inventory;
    [SerializeField] Lookup<ItemData> _itemLookup;
    [SerializeField] MoneyManager _money;
    [SerializeField] FloatDataProperty _buySpeed;
    [SerializeField] int _overbuyAmount;
    [SerializeField] int _buyAmount = 1;
    
    ItemData _item;

    public int OverbuyAmount {
        get => _overbuyAmount;
        set => _overbuyAmount = value;
    }

    public ItemData Item {
        get => _item;
        set {
            _item = value; 
            OnStatusChanged?.Invoke(this);
        }
    }

    public event Action<IStatusProvider> OnStatusChanged;
    
    void OnEnable() {
        StartCoroutine(Routine());
        return;
        
        IEnumerator Routine() {
            while (enabled) {
                if (Item != null) {
                    var count = Mathf.Min(
                        _buyAmount,
                        _overbuyAmount - _inventory.GetCount(Item),
                        (int)(_money.CurrentMoney / Item.Price)
                    );
                    
                    if (count > 0) {
                        _inventory.Add(Item, count);
                        _money.CurrentMoney -= Item.Price * count;
                    }
                }

                yield return CoroutineHelpers.Wait(1 / _buySpeed.Value);
            }
        }
    }
    
    public IEnumerable<EntityInfo> GetInfo() {
        yield return new EntityInfo("Speed", $"{_buySpeed.Value * _buyAmount:0.#}/s");
    }

    public IEnumerable<EntityInfo> GetStatus() {
        yield return new EntityInfo("Item", Item != null ? Item.Name : "None");
    }
    
    public void OnAfterLoad(SaveData data) {
        var id = data.ItemDataId;
        Item = id == -1 ? null : _itemLookup.GetFromId(id);
        OverbuyAmount = data.OverbuyAmount;
    }

    public SaveData GetSaveData() {
        return new SaveData {
            ItemDataId = Item == null ? -1 : _itemLookup.GetId(Item),
            OverbuyAmount = OverbuyAmount
        };
    }

    [Serializable]
    public struct SaveData {
        public int ItemDataId;
        public int OverbuyAmount;

        public override string ToString() {
            return $"ItemDataId: {ItemDataId}, OverbuyAmount: {OverbuyAmount}";
        }
    }
    
    public Component Component => this;
    public FloatDataProperty BoostableProperty => _buySpeed;
}

}