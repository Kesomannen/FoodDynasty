using System;
using Dynasty.Library.Events;
using Dynasty.Library.Extensions;
using UnityEngine;

public class GridObjectMover : MonoBehaviour {
    [SerializeField] GridObjectPlacer _placer;
    [SerializeField] ParticleSystem _placeEffect;
    [SerializeField] GameEvent<GridObject> _startMovingEvent;
    [SerializeField] GameEvent<GridObject> _deleteEvent;

    void OnEnable() {
        _startMovingEvent.AddListener(OnStartMoving);
    }
    
    void OnDisable() {
        _startMovingEvent.RemoveListener(OnStartMoving);
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
        switch (result.ResultType) {
            case GridPlacementResultType.Successful:
                obj.AddAndPosition(_placer.GridManager, result.GridPosition, result.Rotation);
                _placeEffect.transform.position = obj.transform.position;
                _placeEffect.Play();
                break;
            case GridPlacementResultType.Failed:
                oldGridManager.TryAdd(obj, oldPosition, oldRotation);
                break;
            case GridPlacementResultType.Deleted:
                _deleteEvent.Raise(obj);
                Destroy(obj.gameObject);
                break;
            default: throw new ArgumentOutOfRangeException();
        }
    }
}