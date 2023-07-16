using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Dynasty.Core.Grid {

public class GridObjectBuilder : MonoBehaviour {
    [SerializeField] GridObjectPlacer _placer;
    [SerializeField] ParticleSystem _placementParticles;

    public async Task StartPlacing(
        GridObject prefab,
        Func<bool> beforePlace = null, 
        Action<GridObject, GridPlacementResult> afterPlace = null
    ) {
        beforePlace ??= () => true;
        _placer.Cancel();
        
        while (beforePlace()) {
            var (result, gridObject) = await Place(prefab);
            afterPlace?.Invoke(gridObject, result);
            if (!result.WasSuccessful) return;
        }
    }

    async Task<(GridPlacementResult Result, GridObject Object)> Place(GridObject prefab) {
        var result = await _placer.DoPlacement(prefab, false, false);

        if (!result.WasSuccessful) return (result, null);
        
        var gridManager = _placer.GridManager;
        var gridObject = Instantiate(prefab);

        if (!gridObject.AddAndPosition(gridManager, result.GridPosition, result.Rotation))
            throw new Exception("Failed to add grid object even though placement was successful");
        
        _placementParticles.transform.position = gridObject.transform.position;
        _placementParticles.Play();
        return (result, gridObject);
    }
}

}