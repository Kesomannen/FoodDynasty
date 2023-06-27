using System;
using UnityEngine;

public class GridObjectMover : MonoBehaviour {
    [SerializeField] GridObjectPlacer _placer;
    [SerializeField] GameEvent<GridObject> _startMovingEvent;
    [SerializeField] GameEvent<GridObject> _deleteEvent;

    void OnEnable() {
        _startMovingEvent.OnRaised += OnStartMoving;
    }
    
    void OnDisable() {
        _startMovingEvent.OnRaised -= OnStartMoving;
    }

    async void OnStartMoving(GridObject obj) {
        if (_placer.IsPlacing || !obj.CanMove) return;
        
        var oldPosition = obj.GridPosition;
        var oldRotation = obj.Rotation;
        var oldGridManager = obj.GridManager;

        if (!obj.GridManager.TryRemove(obj)) return;
        obj.SetActive(false);
        
        var result = await _placer.DoPlacement(obj);
        
        obj.SetActive(true);
        switch (result.Type) {
            case GridPlacementResultType.Successful:
                obj.AddAndPosition(_placer.GridManager, result.GridPosition, result.Rotation); break;
            case GridPlacementResultType.Failed:
                oldGridManager.TryAdd(obj, oldPosition, oldRotation); break;
            case GridPlacementResultType.Deleted:
                _deleteEvent.Raise(obj);
                Destroy(obj.gameObject);
                break;
            default: throw new ArgumentOutOfRangeException();
        }
    }
}