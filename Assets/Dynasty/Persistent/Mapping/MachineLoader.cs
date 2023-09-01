using System;
using System.Collections.Generic;
using System.Linq;
using Dynasty.Core.Grid;
using Dynasty.Core.Inventory;
using Dynasty.Library.Data;
using UnityEngine;

namespace Dynasty.Persistent.Mapping {

public class MachineLoader : MonoBehaviour {
    [SerializeField] GridManager _gridManager;
    [SerializeField] Lookup<ItemData> _itemLookup;
    
    public Lookup<ItemData> Lookup => _itemLookup;

    public void Load(Data data) {
        for (var i = 0; i < data.ItemIds.Length; i++) {
            var itemData = _itemLookup.GetFromId(data.ItemIds[i]);
            if (itemData is not IPrefabProvider<GridObject> provider) continue;
            
            var gridObject = Instantiate(provider.Prefab);
            if (!gridObject.AddAndPosition(_gridManager, data.Positions[i], data.Rotations[i])) {
                Debug.LogWarning($"Failed to load machine {itemData.Name} at {data.Positions[i]}");
                Destroy(gridObject.gameObject);
                continue;
            }
            
            var savedData = data.AdditionalData[i];
            if (savedData.Length == 0) continue;
            
            var dataComponents = gridObject.GetComponentsInChildren<IAdditionalSaveData>();
            for (var j = 0; j < dataComponents.Length; j++) {
                dataComponents[j].OnAfterLoad(savedData[j]);
            }
        }
    }
    
    public Data Get() {
        var entities = new List<(MachineItemData ItemData, GridObject GridObject)>();
        foreach (var gridObject in _gridManager.GridObjects) {
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

        return new Data {
            ItemIds = entities.Select(tuple => _itemLookup.GetId(tuple.ItemData)).ToArray(),
            Positions = entities.Select(tuple => new SerializableVector2Int(tuple.GridObject.GridPosition)).ToArray(),
            Rotations = entities.Select(tuple => tuple.GridObject.Rotation).ToArray(),
            AdditionalData = additionalData.ToArray()
        };
    }
    
    [Serializable]
    public struct Data {
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
}

}