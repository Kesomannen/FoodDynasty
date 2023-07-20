using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dynasty.Core.Grid;
using Dynasty.Library.Classes;
using Dynasty.Library.Events;
using Dynasty.Library.Extensions;
using Dynasty.Library.Helpers;
using Dynasty.Library.Pooling;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dynasty.Core.Grid {

/// <summary>
/// Handles the placement of <see cref="GridObject"/>s on a <see cref="GridManager"/> using player input.
/// </summary>
public class GridObjectPlacer : MonoBehaviour, IPointerClickHandler {
    [Header("References")] 
    [Tooltip("Used for directional markers.")]
    [SerializeField] CustomObjectPool<Poolable> _arrowPool;

    [SerializeField] GridManager _gridManager;
    
    [Tooltip("The trigger to detect mouse clicks on. Should not be blocked by other colliders.")]
    [SerializeField] Collider _placeTrigger;
    
    [Header("Input")]
    [Tooltip("When raised and the placer is active, rotates the current object.")]
    [SerializeField] GenericGameEvent _rotateEvent;
    
    [Tooltip("When raised and the placer is active, cancels the current placement.")]
    [SerializeField] GenericGameEvent _cancelEvent;
    
    [Tooltip("When raised and the placer is active, deletes the current object.")]
    [SerializeField] GenericGameEvent _deleteEvent;
    
    [Tooltip("Buttons to accept mouse input from.")]
    [SerializeField] PointerEventData.InputButton[] _allowedButtons = { PointerEventData.InputButton.Left };

    [Header("Effects & Animations")]
    [Tooltip("Seconds to move an object to the mouse position.")]
    [SerializeField] float _moveTime = 0.05f;
    
    [Tooltip("Seconds to rotate an object to target rotation.")]
    [SerializeField] float _tweenTime = 0.2f;
    
    [Tooltip("Used when rotating an object.")]
    [SerializeField] LeanTweenType _tweenType;
    
    [Tooltip("Applied when the current placement is valid.")]
    [SerializeField] Material _validMaterial;
    
    [Tooltip("Applied when the current placement is invalid.")]
    [SerializeField] Material _invalidMaterial;

    /// <summary>
    /// Is the placer currently placing an object?
    /// </summary>
    public bool IsPlacing { get; private set; }
    public GridManager GridManager => _gridManager;

    readonly Queue<Poolable> _currentArrows = new();
    
    GridObject _currentObject;
    Transform _currentBlueprint;
    Vector2Int _currentGridPosition;
    GridRotation _currentGridRotation;

    Renderer[] _currentRenderers;
    bool _currentMaterialValidity;
    
    Vector3 _moveTarget;
    Vector3 _moveVelocity;
    Quaternion _rotateTarget;
    int _rotateTweenId;

    Plane _plane;
    State _state;
    
    void Awake() {
        _placeTrigger.enabled = false;
        _plane = new Plane(Vector3.up, transform.position);
    }

    void Update() {
        if (!IsPlacing) return;
        
        _currentGridPosition = GetMouseGridPos();
        UpdateBlueprint(true);
    }
    
    void StartListeningToInput() {
        _rotateEvent.AddListener(OnRotate);
        _deleteEvent.AddListener(OnDelete);
        _cancelEvent.AddListener(Cancel);
    }
    
    void StopListeningToInput() {
        _rotateEvent.RemoveListener(OnRotate);
        _deleteEvent.RemoveListener(OnDelete);
        _cancelEvent.RemoveListener(Cancel);
    }

    public void Cancel() {
        if (!IsPlacing) return;
        _state = State.Cancelled;
    }

    /// <summary>
    /// Starts the placement of a <see cref="GridObject"/>.
    /// </summary>
    /// <param name="data">Used for data gathering. Will not be instantiated.</param>
    /// <param name="keepRotation">Whether or not to begin placement from the data's current rotation.</param>
    /// <param name="keepPosition">Whether or not to begin placement from the data's current position.</param>
    public async Task<GridPlacementResult> DoPlacement(GridObject data, bool keepRotation = true, bool keepPosition = true) {
        if (IsPlacing) {
            return GridPlacementResult.Failed;
        }

        _currentObject = data;
        if (keepRotation) _currentGridRotation = data.Rotation;
        if (keepPosition) _currentGridPosition = data.GridPosition;

        IsPlacing = true;
        _placeTrigger.enabled = true;
        SetupBlueprint(data);
        
        StartListeningToInput();
        await WaitForPlacement(data);
        StopListeningToInput();

        IsPlacing = false;
        _placeTrigger.enabled = false;

        while (_currentArrows.Count > 0) {
            _currentArrows.Dequeue().Dispose();
        }

        LeanTween.cancel(_rotateTweenId);
        Destroy(_currentBlueprint.gameObject);

        return _state switch {
            State.Cancelled => GridPlacementResult.Failed,
            State.Deleted => GridPlacementResult.Deleted,
            State.Placed => new GridPlacementResult(GridPlacementResultType.Successful, _currentGridPosition, _currentGridRotation),
            State.Waiting => throw new Exception("Should not be waiting after placement"),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    void SetupBlueprint(GridObject gridObject) {
        _currentBlueprint = Instantiate(gridObject.BlueprintPrefab).transform;
        
        _moveTarget = Vector3.zero;
        _rotateTarget = Quaternion.identity;

        // Create arrows as direction markers
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
        UpdateBlueprint(false);
    }

    async Task WaitForPlacement(GridObject gridObject) {
        _state = State.Waiting;

        while (true) {
            if (_state != State.Waiting) {
                if (_state is State.Cancelled or State.Deleted) break;

                if (_state == State.Placed) {
                    if (_gridManager.CanAdd(gridObject, _currentGridPosition, _currentGridRotation)) break;
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
        _currentGridRotation += 1;
        UpdateBlueprint(true);
    }

    void UpdateBlueprint(bool tween) {
        MoveBlueprint(tween);
        RotateBlueprint(tween);
    }

    void RotateBlueprint(bool tween) {
        var newRot = _currentGridRotation.ToQuaternion(Vector3.up);
        
        if (_rotateTarget == newRot) return;
        _rotateTarget = newRot;

        if (tween) {
            LeanTween.cancel(_rotateTweenId);
            _rotateTweenId = LeanTween
                .rotate(_currentBlueprint.gameObject, newRot.eulerAngles, _tweenTime)
                .setEase(_tweenType)
                .uniqueId;
        } else {
            _currentBlueprint.rotation = newRot;
        }
    }

    void MoveBlueprint(bool tween) {
        var newTarget = _gridManager.GridToWorld(_currentGridPosition, _currentObject.StaticSize, _currentGridRotation);
        var currentPos = _currentBlueprint.position;

        if (_moveTarget != newTarget) {
            _moveTarget = newTarget;
            UpdateBlueprintValidity();
        }
        
        if (currentPos == _moveTarget) return;
        _currentBlueprint.position = tween ? 
            Vector3.SmoothDamp(currentPos, _moveTarget, ref _moveVelocity, _moveTime)
            : _moveTarget;
    }

    void UpdateBlueprintValidity() {
        var valid = _gridManager.CanAdd(_currentObject, _currentGridPosition, _currentGridRotation);
        if (valid == _currentMaterialValidity) return;

        _currentMaterialValidity = valid;
        _currentRenderers.ApplyMaterial(valid ? _validMaterial : _invalidMaterial);
    }

    Vector3 GetMouseWorldPos() {
        var ray = MouseHelpers.MainCamera.ScreenPointToRay(MouseHelpers.ScreenPosition);
        return _plane.Raycast(ray, out var enter) ? ray.GetPoint(enter) : Vector3.zero;
    }
    
    Vector2Int GetMouseGridPos() {
        var currentBounds = _currentObject.StaticSize.Rotated(_currentGridRotation.Steps).Bounds;
        return _gridManager.WorldToGrid(GetMouseWorldPos()) - currentBounds / 2;
    }

    enum State {
        Waiting,
        Placed,
        Cancelled,
        Deleted
    }
}

}

public readonly struct GridPlacementResult {
    public readonly Vector2Int GridPosition;
    public readonly GridRotation Rotation;
    public readonly GridPlacementResultType ResultType;
    public bool WasSuccessful => ResultType == GridPlacementResultType.Successful;
    
    public GridPlacementResult(GridPlacementResultType resultType, Vector2Int gridPosition = default, GridRotation rotation = default) {
        GridPosition = gridPosition;
        Rotation = rotation;
        ResultType = resultType;
    }
    
    public static implicit operator bool(GridPlacementResult result) => result.WasSuccessful;
    
    public static GridPlacementResult Failed => new(GridPlacementResultType.Failed);
    public static GridPlacementResult Deleted => new(GridPlacementResultType.Deleted);
}

public enum GridPlacementResultType {
    Successful,
    Failed,
    Deleted
}