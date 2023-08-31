using System;
using System.Collections.Generic;
using System.Linq;
using Dynasty.Library.Data;
using Dynasty.Core.Grid;
using Dynasty.Core.Inventory;
using Dynasty.Persistent.Core;
using Dynasty.Persistent.Mapping;
using UnityEngine;

namespace Dynasty.Persistent.Mapping {

public class MachinesSaveStore : SaveStore<MachinesSaveStore.SaveData> {
    [Space]
    [SerializeField] GridManager _gridManager;
    [SerializeField] Lookup<ItemData> _itemLookup;
    [Space]
    [SerializeField] StartingMachine[] _startingMachines;

    protected override SaveData GetDefaultData() {
        return new SaveData {
            ItemIds = _startingMachines.Select(machine => _itemLookup.GetId(machine.ItemData)).ToArray(),
            Positions = _startingMachines.Select(machine => machine.Position).ToArray(),
            Rotations = _startingMachines.Select(machine => machine.Rotation).ToArray(),
            AdditionalData = _startingMachines.Select(_ => Array.Empty<object>()).ToArray()
        };
    }

    protected override void OnAfterLoad(SaveData saveData) {
        for (var i = 0; i < saveData.ItemIds.Length; i++) {
            var itemData = _itemLookup.GetFromId(saveData.ItemIds[i]);
            if (itemData is not IPrefabProvider<GridObject> provider) continue;
            
            var gridObject = Instantiate(provider.Prefab);
            if (!gridObject.AddAndPosition(_gridManager, saveData.Positions[i], saveData.Rotations[i])) {
                Debug.LogWarning($"Failed to load machine {itemData.Name} at {saveData.Positions[i]}");
                Destroy(gridObject.gameObject);
                continue;
            }
            
            var savedData = saveData.AdditionalData[i];
            if (savedData.Length == 0) continue;
            
            var dataComponents = gridObject.GetComponentsInChildren<IAdditionalSaveData>();
            for (var j = 0; j < dataComponents.Length; j++) {
                dataComponents[j].OnAfterLoad(savedData[j]);
            }
        }
    }

    protected override SaveData GetSaveData() {
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

        return new SaveData {
            ItemIds = entities.Select(tuple => _itemLookup.GetId(tuple.ItemData)).ToArray(),
            Positions = entities.Select(tuple => new SerializableVector2Int(tuple.GridObject.GridPosition)).ToArray(),
            Rotations = entities.Select(tuple => tuple.GridObject.Rotation).ToArray(),
            AdditionalData = additionalData.ToArray()
        };
    }
    
    [Serializable]
    struct StartingMachine {
        [SerializeField] MachineItemData _itemData;
        [SerializeField] Vector2Int _position;
        [SerializeField] GridRotation _rotation;
        
        public MachineItemData ItemData => _itemData;
        public SerializableVector2Int Position => new(_position);
        public GridRotation Rotation => _rotation;
    }
    
    [Serializable]
    public struct SaveData {
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