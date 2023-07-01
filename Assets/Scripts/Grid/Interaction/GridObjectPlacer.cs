using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridObjectPlacer : MonoBehaviour, IPointerClickHandler {
    [Header("References")] 
    [SerializeField] CustomObjectPool<Poolable> _arrowPool;
    [SerializeField] GridManager _gridManager;
    [SerializeField] Collider _placeTrigger;
    
    [Header("Input")]
    [SerializeField] GenericGameEvent _rotateEvent;
    [SerializeField] GenericGameEvent _cancelEvent;
    [SerializeField] GenericGameEvent _deleteEvent;
    [SerializeField] PointerEventData.InputButton[] _allowedButtons = { PointerEventData.InputButton.Left };
    
    [Header("Effects & Animations")]
    [SerializeField] Material _validMaterial;
    [SerializeField] Material _invalidMaterial;

    public bool IsPlacing { get; private set; }
    public GridManager GridManager => _gridManager;

    readonly Queue<Poolable> _currentArrows = new();
    
    GridObject _currentObject;
    Transform _currentBlueprint;
    Vector2Int _currentPosition;
    GridRotation _currentRotation;

    Renderer[] _currentRenderers;
    bool _currentMaterialValidity;
    
    Plane _plane;
    State _state;
    
    void Awake() {
        _placeTrigger.enabled = false;
        _plane = new Plane(Vector3.up, Vector3.zero);
    }

    void Update() {
        if (!IsPlacing) return;
        _currentPosition = GetMouseGridPos();
        UpdateBlueprint();
    }

    void OnEnable() {
        _rotateEvent.OnRaisedGeneric += OnRotate;
        _deleteEvent.OnRaisedGeneric += OnDelete;
        _cancelEvent.OnRaisedGeneric += Cancel;
    }
    
    void OnDisable() {
        _rotateEvent.OnRaisedGeneric -= OnRotate;
        _deleteEvent.OnRaisedGeneric -= OnDelete;
        _cancelEvent.OnRaisedGeneric -= Cancel;
    }

    public void Cancel() {
        if (!IsPlacing) return;
        _state = State.Cancelled;
    }

    public async Task<GridPlacementResult> DoPlacement(GridObject data, bool keepRotation = true, bool keepPosition = true) {
        if (IsPlacing) {
            return GridPlacementResult.Failed;
        }
        
        _currentObject = data;
        if (keepRotation) _currentRotation = data.Rotation;
        if (keepPosition) _currentPosition = data.GridPosition;

        IsPlacing = true;
        _placeTrigger.enabled = true;
        SetupBlueprint(data);
        
        await WaitForPlacement(data);

        IsPlacing = false;
        _placeTrigger.enabled = false;

        while (_currentArrows.Count > 0) {
            _currentArrows.Dequeue().Dispose();
        }
        Destroy(_currentBlueprint.gameObject);

        return _state switch {
            State.Cancelled => GridPlacementResult.Failed,
            State.Deleted => GridPlacementResult.Deleted,
            State.Placed => new GridPlacementResult (_currentPosition, _currentRotation, GridPlacementResultType.Successful),
            State.Waiting => throw new Exception("Should not be waiting after placement"),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    void SetupBlueprint(GridObject gridObject) {
        _currentBlueprint = Instantiate(gridObject.BlueprintPrefab).transform;
        
        foreach (var child in gridObject.transform.GetChildren()) {
            if (!child.CompareTag("DirectionMarker")) continue;
            if (!child.gameObject.activeSelf) continue;
            
            var arrow = _arrowPool.Get();
            
            var arrowTransform = arrow.transform;
            arrowTransform.SetParent(_currentBlueprint, false);
            arrowTransform.SetLocalPositionAndRotation(child.localPosition, child.localRotation);
            
            _currentArrows.Enqueue(arrow);
        }
        
        _currentRenderers = _currentBlueprint.GetComponentsInChildren<Renderer>();
        _currentRenderers.ApplyMaterial(_validMaterial);
        
        _currentMaterialValidity = true;
        UpdateBlueprint();
    }

    async Task WaitForPlacement(GridObject gridObject) {
        _state = State.Waiting;

        while (true) {
            if (_state != State.Waiting) {
                if (_state is State.Cancelled or State.Deleted) break;

                if (_state == State.Placed) {
                    if (_gridManager.CanAdd(gridObject, _currentPosition, _currentRotation)) break;
                    _state = State.Waiting;
                }
            }
            await Task.Yield();
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (!IsPlacing) return;
        if (!_allowedButtons.Contains(eventData.button)) return;
        _state = State.Placed;
    }

    void OnDelete() {
        if (!IsPlacing) return;
        _state = State.Deleted;
    }

    void OnRotate() {
        if (!IsPlacing) return;
        _currentRotation += 1;
        UpdateBlueprint();
    }

    void UpdateBlueprint() {
        _currentBlueprint.rotation = _currentRotation.ToQuaternion(Vector3.up);
        
        var newPos = _gridManager.GridToWorld(_currentPosition, _currentObject.StaticSize, _currentRotation);
        if (_currentBlueprint.position != newPos) {
            UpdateBlueprintValidity();
        }
        
        _currentBlueprint.position = newPos;
    }

    void UpdateBlueprintValidity() {
        var valid = _gridManager.CanAdd(_currentObject, _currentPosition, _currentRotation);
        if (valid == _currentMaterialValidity) return;

        _currentMaterialValidity = valid;
        _currentRenderers.ApplyMaterial(valid ? _validMaterial : _invalidMaterial);
    }

    Vector3 GetMouseWorldPos() {
        var ray = MouseHelpers.MainCamera.ScreenPointToRay(MouseHelpers.ScreenPosition);
        return _plane.Raycast(ray, out var enter) ? ray.GetPoint(enter) : Vector3.zero;
    }
    
    Vector2Int GetMouseGridPos() {
        var currentBounds = _currentObject.StaticSize.Rotated(_currentRotation.Steps).Bounds;
        return _gridManager.WorldToGrid(GetMouseWorldPos()) - currentBounds / 2;
    }

    enum State {
        Waiting,
        Placed,
        Cancelled,
        Deleted
    }
}

public readonly struct GridPlacementResult {
    public readonly Vector2Int GridPosition;
    public readonly GridRotation Rotation;
    public readonly GridPlacementResultType ResultType;
    public bool WasSuccessful => ResultType == GridPlacementResultType.Successful;
    
    public GridPlacementResult(Vector2Int gridPosition, GridRotation rotation, GridPlacementResultType resultType) {
        GridPosition = gridPosition;
        Rotation = rotation;
        ResultType = resultType;
    }
    
    public static implicit operator bool(GridPlacementResult result) => result.WasSuccessful;
    
    public static GridPlacementResult Failed => new(Vector2Int.zero, new GridRotation(), GridPlacementResultType.Failed);
    public static GridPlacementResult Deleted => new(Vector2Int.zero, new GridRotation(), GridPlacementResultType.Deleted);
}

public enum GridPlacementResultType {
    Successful,
    Failed,
    Deleted
}