using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Dynasty.Grid {

/// <summary>
/// Provides an abstraction for placing <see cref="GridObject"/>s.
/// </summary>
public class GridObjectBuilder : MonoBehaviour {
    [SerializeField] GridObjectPlacer _placer;
    
    [Tooltip("Played when a grid object is placed.")]
    [SerializeField] ParticleSystem _placementParticles;

    /// <summary>
    /// Starts placing the given prefab until the given callback returns false or the user cancels.
    /// </summary>
    /// <param name="prefab">The grid object prefab to place.</param>
    /// <param name="beforePlace">Return false to stop placement. Defaults to true.</param>
    /// <param name="afterPlace">Called after any placement is made, successful or not.</param>
    public async Task StartPlacing(
        GridObject prefab,
        Func<bool> beforePlace = null, 
        Action<GridObject, GridPlacementResult> afterPlace = null
    ) {
        beforePlace ??= () => true;
        _placer.Cancel();

        await Task.Yield();
        
        while (beforePlace()) {
            var (result, gridObject) = await Place(prefab);
            afterPlace?.Invoke(gridObject, result);
            if (!result.WasSuccessful) return;
        }
    }
    
    async Task<(GridPlacementResult Result, GridObject Object)> Place(GridObject prefab) {
        var result = await _placer.DoPlacement(prefab, false);

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