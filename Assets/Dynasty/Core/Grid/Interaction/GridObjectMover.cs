using System;
using System.Collections.Generic;
using System.Linq;
using Dynasty.Library;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dynasty.Grid {

/// <summary>
/// Allows moving a <see cref="GridObject"/> by placing it again.
/// </summary>
public class GridObjectMover : MonoBehaviour {
    [SerializeField] GridObjectPlacer _placer;
    
    [Tooltip("Played when a grid object is placed.")]
    [SerializeField] ParticleSystem _placeEffect;
    
    [Tooltip("When raised, starts moving the given grid object.")]
    [SerializeField] GameEvent<GridObject> _startMovingEvent;
    
    [FormerlySerializedAs("_deleteEvent")]
    [Tooltip("Raised when a grid object is deleted.")]
    [SerializeField] GameEvent<GridObject> _onDelete;

    void OnEnable() {
        _startMovingEvent.AddListener(OnStartMovingHandler);
    }
    
    void OnDisable() {
        _startMovingEvent.RemoveListener(OnStartMovingHandler);
    }
    
    void OnStartMovingHandler(GridObject obj) {
        StartMoving(new[] { obj });
    }
    
    public async void StartMoving(IEnumerable<GridObject> objects) {
        if (_placer.IsPlacing) return;
        
        var list = objects.Where(o => o.CanMove && o.GridManager != null).ToList();
        if (list.Count == 0) return;
        
        var oldPositions = list.Select(o => o.GridPosition).ToList();
        var oldRotations = list.Select(o => o.Rotation).ToList();
        var oldGridManagers = list.Select(o => o.GridManager).ToList();

        for (var i = 0; i < list.Count; i++) {
            var obj = list[i];
            if (!obj.GridManager!.TryRemove(obj)) {
                list.RemoveAt(i);
                oldPositions.RemoveAt(i);
                oldRotations.RemoveAt(i);
                oldGridManagers.RemoveAt(i);
                i--;
                
                continue;
            }
            obj.SetActive(false);
        }
        
        if (list.Count == 0) return;
        
        var results = await _placer.DoPlacement(list.ToArray());

        for (var i = 0; i < list.Count; i++) {
            var obj = list[i];
            var result = results[i];
            obj.SetActive(true);

            switch (result.ResultType) {
                case GridPlacementResultType.Successful:
                    obj.AddAndPosition(_placer.GridManager, result.GridPosition, result.Rotation);
                    _placeEffect.transform.position = obj.transform.position;
                    _placeEffect.Play();
                    break;
                case GridPlacementResultType.Failed:
                    if (!oldGridManagers[i].TryAdd(obj, oldPositions[i], oldRotations[i])) {
                        Debug.LogError($"Failed to re-add {obj.name} to its original position.");
                    }
                    break;
                case GridPlacementResultType.Deleted:
                    _onDelete.Raise(obj);
                    Destroy(obj.gameObject);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}

}