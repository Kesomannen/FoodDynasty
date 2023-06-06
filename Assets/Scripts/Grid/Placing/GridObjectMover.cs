using UnityEngine;

public class GridObjectMover : MonoBehaviour {
    [SerializeField] GridObjectPlacer _placer;
    [SerializeField] GameEvent<GridObject> _gridObjectClickedEvent;

    void OnEnable() {
        _gridObjectClickedEvent.AddListener(OnGridObjectClicked);
    }
    
    void OnDisable() {
        _gridObjectClickedEvent.RemoveListener(OnGridObjectClicked);
    }

    async void OnGridObjectClicked(GridObject obj) {
        if (_placer.IsPlacing) return;
        
        var oldPosition = obj.GridPosition;
        var oldRotation = obj.Rotation;
        var oldGridManager = obj.GridManager;

        if (!obj.GridManager.TryRemove(obj)) return;
        obj.SetActive(false);
        
        var result = await _placer.DoPlacement(obj);
        
        obj.SetActive(true);
        if (result.Succeeded) {
            obj.AddAndPosition(_placer.GridManager, result.GridPosition, result.Rotation);
        } else {
            oldGridManager.TryAdd(obj, oldPosition, oldRotation);
        }
    }
}