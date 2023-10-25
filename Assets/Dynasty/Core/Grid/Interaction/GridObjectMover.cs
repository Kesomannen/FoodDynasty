using System;
using Dynasty.Core.Grid;
using Dynasty.Library;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dynasty.Core.Grid {

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
        _startMovingEvent.AddListener(OnStartMoving);
    }
    
    void OnDisable() {
        _startMovingEvent.RemoveListener(OnStartMoving);
    }

    async void OnStartMoving(GridObject obj) {
        if (_placer.IsPlacing || !obj.CanMove || obj.GridManager == null) return;
        
        var oldPosition = obj.GridPosition;
        var oldRotation = obj.Rotation;
        var oldGridManager = obj.GridManager;

        if (!obj.GridManager.TryRemove(obj)) return;
        obj.SetActive(false);
        
        var result = await _placer.DoPlacement(obj);
        
        obj.SetActive(true);
        switch (result.ResultType) {
            case GridPlacementResultType.Successful:
                obj.AddAndPosition(_placer.GridManager, result.GridPosition, result.Rotation);
                _placeEffect.transform.position = obj.transform.position;
                _placeEffect.Play();
                return;
            case GridPlacementResultType.Failed:
                oldGridManager.TryAdd(obj, oldPosition, oldRotation);
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