using System;
using System.Linq;
using Dynasty;
using UnityEngine;

namespace Dynasty.Persistent {

[CreateAssetMenu(menuName = "Saving/Interpreter/Inventory")]
public class InventorySaveInterpreter : SaveInterpreter<InventorySaveData> {
    [SerializeField] InventoryAsset _inventoryAsset;
    [SerializeField] Lookup<ItemData> _itemLookup;
    [SerializeField] ItemData[] _startingItems;

    protected override InventorySaveData DefaultData => new() { 
        ItemIds = _startingItems.Select(item => _itemLookup.GetId(item)).ToArray(),
        ItemCounts = _startingItems.Select(_ => 1).ToArray()
    };

    protected override void OnLoad(InventorySaveData saveData) {
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