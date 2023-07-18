﻿using System;
using System.Linq;
using Dynasty.Core.Inventory;
using Dynasty.Persistent.Core;
using UnityEngine;

namespace Dynasty.Persistent.Mapping {

[CreateAssetMenu(menuName = "Saving/Interpreter/Inventory")]
public class InventorySaveInterpreter : SaveInterpreter<InventorySaveData> {
    [SerializeField] InventoryAsset _inventoryAsset;
    [SerializeField] Lookup<ItemData> _itemLookup;

    protected override InventorySaveData DefaultValue => 
        new() { ItemIds = Array.Empty<int>(), ItemCounts = Array.Empty<int>() };

    protected override void OnAfterLoad(InventorySaveData saveData) {
        for (var i = 0; i < saveData.ItemIds.Length; i++) {
            _inventoryAsset.Add(_itemLookup.GetFromId(saveData.ItemIds[i]), saveData.ItemCounts[i]);
        }
    }

    protected override InventorySaveData GetSaveData() {
        return new InventorySaveData {
            ItemIds = _inventoryAsset.Items.Select(item => _itemLookup.GetId(item.Data)).ToArray(),
            ItemCounts = _inventoryAsset.Items.Select(item => item.Count).ToArray()
        };
    }
}

}

[Serializable]
public struct InventorySaveData {
    public int[] ItemIds;
    public int[] ItemCounts;

    public override string ToString() {
        return $"ItemIds: {string.Join(", ", ItemIds)}\n" +
               $"ItemCounts: {string.Join(", ", ItemCounts)}";
    }
}