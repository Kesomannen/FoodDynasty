using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GridObjectBuilder : MonoBehaviour {
    [SerializeField] GridObjectPlacer _placer;

    public async void StartPlacing(GridObject prefab) {
        var placedObjects = new List<GridObject>();
        _placer.Cancel();
        
        while (true) {
            var (succeeded, obj) = await Place(prefab);
            if (!succeeded) break;
            placedObjects.Add(obj);
        }
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