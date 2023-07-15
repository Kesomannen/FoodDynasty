using System;
using System.Collections.Generic;
using System.Linq;
using Dynasty.Core.Data;
using Dynasty.Core.Grid;
using Dynasty.Core.Inventory;
using Dynasty.Persistent.Core;
using Dynasty.Persistent.Mapping;
using UnityEngine;

namespace Dynasty.Persistent.Mapping {

public class MachinesSaveStore : SaveStore<MachinesSaveData> {
    [Space]
    [SerializeField] GridManager _gridManager;
    [SerializeField] Lookup<ItemData> _itemLookup;

    protected override MachinesSaveData DefaultValue => new() {
        ItemIds = Array.Empty<int>(),
        Positions = Array.Empty<SerializableVector2Int>(),
        Rotations = Array.Empty<GridRotation>(),
        AdditionalData = Array.Empty<object[]>()
    };
    
    protected override void OnAfterLoad(MachinesSaveData saveData) {
        for (var i = 0; i < saveData.ItemIds.Length; i++) {
            var itemData = _itemLookup.GetFromId(saveData.ItemIds[i]);
            if (itemData is not IPrefabProvider<GridObject> provider) continue;
            
            var gridObject = Instantiate(provider.Prefab);
            gridObject.AddAndPosition(_gridManager, saveData.Positions[i], saveData.Rotations[i]);
            
            var dataComponents = gridObject.GetComponentsInChildren<IAdditionalSaveData>();
            if (dataComponents.Length == 0) continue;
            
            var savedData = saveData.AdditionalData[i];
            for (var j = 0; j < dataComponents.Length; j++) {
                dataComponents[j].OnAfterLoad(savedData[j]);
            }
        }
    }

    protected override MachinesSaveData GetSaveData() {
        var entities = new List<(MachineItemData ItemData, GridObject GridObject)>();
        foreach (var gridObject in _gridManager.GetAllObjects()) {
            if (!gridObject.TryGetComponent(out MachineEntity entity)) continue;
            entities.Add((entity.Data, gridObject));
        }

        var additionalData = new List<object[]>();
        foreach (var (_, gridObject) in entities) {
            var dataComponents = gridObject.GetComponentsInChildren<IAdditionalSaveData>();
            if (dataComponents.Length == 0) {
                additionalData.Add(Array.Empty<object>());
                continue;
            }
            
            var saveData = new object[dataComponents.Length];
            for (var i = 0; i < dataComponents.Length; i++) {
                saveData[i] = dataComponents[i].GetSaveData();
            }
            
            additionalData.Add(saveData);
        }

        return new MachinesSaveData {
            ItemIds = entities.Select(tuple => _itemLookup.GetId(tuple.ItemData)).ToArray(),
            Positions = entities.Select(tuple => new SerializableVector2Int(tuple.GridObject.GridPosition)).ToArray(),
            Rotations = entities.Select(tuple => tuple.GridObject.Rotation).ToArray(),
            AdditionalData = additionalData.ToArray()
        };
    }
}

}

[Serializable]
public struct MachinesSaveData {
    public int[] ItemIds;
    public SerializableVector2Int[] Positions;
    public GridRotation[] Rotations;
    public object[][] AdditionalData;
    
    public override string ToString() {
        return $"ItemIds: {string.Join(", ", ItemIds)}\n" +
               $"Positions: {string.Join(", ", Positions)}\n" +
               $"Rotations: {string.Join(", ", Rotations)}\n" +
               $"AdditionalData: {string.Join(", ", AdditionalData.Select(data => $"[{string.Join(", ", data)}]"))}";
    }
}