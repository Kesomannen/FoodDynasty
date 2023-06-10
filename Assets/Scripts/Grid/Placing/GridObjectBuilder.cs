using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GridObjectBuilder : MonoBehaviour {
    [SerializeField] GridObjectPlacer _placer;

    public void Cancel() {
        _placer.Cancel();
    }
    
    public async Task<List<GridObject>> StartPlacing(GridObject prefab, Action<GridObject> onPlaced = null) {
        var placedObjects = new List<GridObject>();
        _placer.Cancel();
        
        while (true) {
            var (succeeded, obj) = await Place(prefab);
            if (!succeeded) break;
            
            placedObjects.Add(obj);
            onPlaced?.Invoke(obj);
        }
        
        return placedObjects;
    }

    async Task<(bool Succeeded, GridObject Object)> Place(GridObject prefab) {
        var result = await _placer.DoPlacement(prefab, false, false);

        if (!result.Succeeded) return (false, null);
        
        var gridManager = _placer.GridManager;
        var gridObject = Instantiate(prefab);

        if (gridObject.AddAndPosition(gridManager, result.GridPosition, result.Rotation)) {
            return (true, gridObject);
        }
        
        Destroy(gridObject);
        return (false, null);
    }
}