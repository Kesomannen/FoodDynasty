using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GridObjectBuilder : MonoBehaviour {
    [SerializeField] GridObjectPlacer _placer;

    public void Cancel() {
        _placer.Cancel();
    }
    
    public async Task<List<GridObject>> StartPlacing(GridObject prefab, Func<bool> beforePlace = null, Action<GridObject, GridPlacementResult> afterPlace = null) {
        var placedObjects = new List<GridObject>();
        beforePlace ??= () => true;
        _placer.Cancel();
        
        while (true) {
            if (!beforePlace()) break;
            
            var (result, obj) = await Place(prefab);
            afterPlace?.Invoke(obj, result);
            if (!result.Succeeded) break;
            
            placedObjects.Add(obj);
        }
        
        return placedObjects;
    }

    async Task<(GridPlacementResult Result, GridObject Object)> Place(GridObject prefab) {
        var result = await _placer.DoPlacement(prefab, false, false);

        if (!result.Succeeded) return (result, null);
        
        var gridManager = _placer.GridManager;
        var gridObject = Instantiate(prefab);

        if (gridObject.AddAndPosition(gridManager, result.GridPosition, result.Rotation)) {
            return (result, gridObject);
        }
        
        Destroy(gridObject);
        result.Succeeded = false;
        return (result, null);
    }
}