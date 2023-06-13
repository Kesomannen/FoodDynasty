using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridObjectPlacer : MonoBehaviour, IPointerClickHandler, IPointerMoveHandler {
    [Header("References")]
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
    public IGridManager GridManager => _gridManager;

    IGridObject _currentObject;
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

    public async Task<GridPlacementResult> DoPlacement(IGridObject data, bool keepRotation = true, bool keepPosition = true) {
        if (IsPlacing) {
            return new GridPlacementResult { Type = GridPlacementResultType.Failed };
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
        Destroy(_currentBlueprint.gameObject);

        return _state switch {
            State.Cancelled => new GridPlacementResult { Type = GridPlacementResultType.Failed },
            State.Deleted => new GridPlacementResult { Type = GridPlacementResultType.Deleted },
            State.Placed => new GridPlacementResult {
                GridPosition = _currentPosition,
                Rotation = _currentRotation,
                Type = GridPlacementResultType.Successful
            },
            State.Waiting => throw new InvalidOperationException(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    void SetupBlueprint(IGridObject gridObject) {
        _currentBlueprint = Instantiate(gridObject.BlueprintPrefab).transform;
        
        _currentRenderers = _currentBlueprint.GetComponentsInChildren<Renderer>();
        _currentRenderers.ApplyMaterial(_validMaterial);
        
        _currentMaterialValidity = true;
        UpdateBlueprint();
    }

    async Task WaitForPlacement(IGridObject gridObject) {
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

    public void OnPointerMove(PointerEventData eventData) {
        if (!IsPlacing) return;
        _currentPosition = GetGridPos(eventData);
        UpdateBlueprint();
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

    Vector3 GetWorldPos(PointerEventData eventData) {
        var ray = MouseHelpers.MainCamera.ScreenPointToRay(eventData.position);
        return _plane.Raycast(ray, out var enter) ? ray.GetPoint(enter) : Vector3.zero;
    }
    
    Vector2Int GetGridPos(PointerEventData eventData) {
        return _gridManager.WorldToGrid(GetWorldPos(eventData));
    }

    enum State {
        Waiting,
        Placed,
        Cancelled,
        Deleted
    }
}

public struct GridPlacementResult {
    public Vector2Int GridPosition;
    public GridRotation Rotation;
    public GridPlacementResultType Type;
    public bool WasSuccessful => Type == GridPlacementResultType.Successful;
}

public enum GridPlacementResultType {
    Successful,
    Failed,
    Deleted
}