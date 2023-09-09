using System;
using System.Collections.Generic;
using System.Linq;
using Dynasty.Core.Grid;
using Dynasty.Core.Inventory;
using Dynasty.Library.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace Dynasty.Persistent.Mapping {

public class MachineLoader : MonoBehaviour {
    [SerializeField] GridManager _gridManager;
    [SerializeField] Lookup<ItemData> _itemLookup;
    
    public Lookup<ItemData> Lookup => _itemLookup;

    public void Load(MachineSaveData data) {
        for (var i = 0; i < data.ItemIds.Length; i++) {
            var itemData = _itemLookup.GetFromId(data.ItemIds[i]);
            if (itemData is not IPrefabProvider<GridObject> provider) continue;
            
            var gridObject = Instantiate(provider.Prefab);
            if (!gridObject.AddAndPosition(_gridManager, data.Positions[i], data.Rotations[i])) {
                Debug.LogWarning($"Failed to load machine {itemData.Name} at {data.Positions[i]}");
                Destroy(gridObject.gameObject);
                continue;
            }
            
            var savedData = data.AdditionalData[i].Data;
            if (savedData.Length == 0) continue;
            
            var dataComponents = gridObject.GetComponentsInChildren<IAdditionalSaveData>();
            for (var j = 0; j < dataComponents.Length; j++) {
                dataComponents[j].OnAfterLoad(savedData[j]);
            }
        }
    }
    
    public MachineSaveData Get() {
        var objects = new List<(MachineItemData ItemData, GridObject GridObject)>();
        foreach (var gridObject in _gridManager.GridObjects) {
            if (!gridObject.TryGetComponent(out MachineEntity entity)) continue;
            objects.Add((entity.Data, gridObject));
        }

        var additionalData = new List<MachineSaveData.AdditionalDataItem>();
        foreach (var (_, gridObject) in objects) {
            var dataComponents = gridObject.GetComponentsInChildren<IAdditionalSaveData>();
            
            additionalData.Add(new MachineSaveData.AdditionalDataItem {
                Data = dataComponents
                    .Select(component => JsonConvert.SerializeObject(component.GetSaveData()))
                    .ToArray()
            });
        }

        return new MachineSaveData {
            ItemIds = objects.Select(tuple => _itemLookup.GetId(tuple.ItemData)).ToArray(),
            Positions = objects.Select(tuple => new SerializableVector2Int(tuple.GridObject.GridPosition)).ToArray(),
            Rotations = objects.Select(tuple => tuple.GridObject.Rotation).ToArray(),
            AdditionalData = additionalData.ToArray()
        };
    }

    public void Clear() {
        Clear(Destroy);
    }

    public void ClearImmediate() {
        Clear(DestroyImmediate);
    }

    void Clear(Action<GameObject> destroy) {
        var gridObjects = _gridManager.GridObjects.ToArray();
        foreach (var gridObject in gridObjects) {
            if (_gridManager.TryRemove(gridObject)) {
                destroy(gridObject.gameObject);
            }
        }
    }
}

[Serializable]
public class MachineSaveData {
    public int[] ItemIds;
    public SerializableVector2Int[] Positions;
    public GridRotation[] Rotations;
    public AdditionalDataItem[] AdditionalData;
    
    public override string ToString() {
        return $"ItemIds: {string.Join(", ", ItemIds)}\n" +
               $"Positions: {string.Join(", ", Positions)}\n" +
               $"Rotations: {string.Join(", ", Rotations)}\n" +
               $"AdditionalData: {string.Join(", ", AdditionalData.Select(data => $"[{string.Join(", ", data)}]"))}";
    }

    [Serializable]
    public class AdditionalDataItem {
        public string[] Data = Array.Empty<string>();
        
        public override string ToString() {
            return $"[{string.Join(", ", Data)}]";
        }
    }
}

}