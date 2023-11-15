using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dynasty.Grid;
using Dynasty.Library;
using Dynasty.UI;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Dynasty.Grid {

/// <summary>
/// Handles the placement of <see cref="GridObject"/>s on a <see cref="GridManager"/> using player input.
/// </summary>
public class GridObjectPlacer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    [Header("References")] 
    [Tooltip("Used for directional markers.")]
    [SerializeField] CustomObjectPool<Poolable> _arrowPool;
    
    [Tooltip("Used for range indicators.")]
    [SerializeField] CustomObjectPool<Poolable> _rangeIndicatorPool;

    [SerializeField] GridManager _gridManager;
    
    [Tooltip("The trigger to detect mouse clicks on. Should not be blocked by other colliders.")]
    [SerializeField] Collider _placeTrigger;
    
    [Header("Input")]
    [Tooltip("When raised and the placer is active, rotates the current object.")]
    [SerializeField] InputEvent _rotateEvent;
    
    [Tooltip("When raised and the placer is active, cancels the current placement.")]
    [SerializeField] InputEvent _cancelEvent;
    
    [Tooltip("When raised and the placer is active, deletes the current object.")]
    [SerializeField] InputEvent _deleteEvent;
    
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

    [Layer]
    [SerializeField] int _ignoreRaycastLayer;
    
    [SerializeField] GameEvent<CameraShake> _cameraShakeEvent;
    [SerializeField] CameraShake _placeShake;
    
    /// <summary>
    /// Is the placer currently placing an object?
    /// </summary>
    public bool IsPlacing { get; private set; }
    public GridManager GridManager => _gridManager;

    readonly Queue<Poolable> _arrows = new();
    readonly Queue<Poolable> _rangeIndicators = new();
    readonly List<PlacingObject> _placingObjects = new();
    readonly List<Renderer> _renderers = new();
    
    bool _materialValidity;
    Vector2Int _gridPosition;
    GridSize _combinedSize;
    Vector2Int _moveAxis;

    GridRotation _lastRotation;

    Plane _plane;
    State _state;

    bool _isHolding;
    
    void Awake() {
        _placeTrigger.enabled = false;
        _plane = new Plane(Vector3.up, transform.position);
    }

    void Update() {
        if (!IsPlacing) return;
        
        UpdateGridPosition();
        UpdateBlueprints(true);
    }
    
    void StartListeningToInput() {
        _rotateEvent.AddListener(OnRotate);
        _deleteEvent.AddListener(OnDelete);
        _cancelEvent.AddListener(Cancel);
        
        Keybinds.Activate(_rotateEvent, _deleteEvent, _cancelEvent);
    }
    
    void StopListeningToInput() {
        _rotateEvent.RemoveListener(OnRotate);
        _deleteEvent.RemoveListener(OnDelete);
        _cancelEvent.RemoveListener(Cancel);
        
        Keybinds.Deactivate(_rotateEvent, _deleteEvent, _cancelEvent);
    }

    public void Cancel() {
        if (IsPlacing) {
            StopPlacing();
        }
    }
    
    public async Task<GridPlacementResult> DoPlacement(GridObject data, bool keepRotation = true) {
        return (await DoPlacement(new[] { data }, keepRotation))[0];
    }
    
    public async Task<GridPlacementResult[]> DoPlacement(GridObject[] data, bool keepRotations = true) {
        if (IsPlacing) {
            return Fail();
        }
        
        _placingObjects.Clear();
        _renderers.Clear();
        
        var max = new Vector2Int(int.MinValue, int.MinValue);
        var min = new Vector2Int(int.MaxValue, int.MaxValue);
        
        var blockingPositions = data.SelectMany(obj => obj.RotatedSize.GetBlockingPositions()
            .Select(pos => pos + obj.GridPosition));
        
        foreach (var pos in blockingPositions) {
            max = Vector2Int.Max(max, pos);
            min = Vector2Int.Min(min, pos);
        }
        
        _combinedSize = new GridSize(max - min + Vector2Int.one, GridSizeType.Custom, false);
        
        foreach (var obj in data) {
            var placingObject = SetupObject(obj, keepRotations);
            placingObject.Offset = obj.GridPosition - min;
            foreach (var pos in placingObject.GridObject.RotatedSize.GetBlockingPositions()) {
                _combinedSize.Set(pos + placingObject.Offset, true);
            }
            _placingObjects.Add(placingObject);
        }
        
        UpdateGridPosition();
        _materialValidity = true;
        UpdateBlueprints(false);
        
        IsPlacing = true;
        _placeTrigger.enabled = true;
        
        StartListeningToInput();
        await WaitForPlacement();

        return IsPlacing ? StopPlacing().ToArray() : Fail();

        GridPlacementResult[] Fail() => Enumerable.Repeat(GridPlacementResult.Failed, data.Length).ToArray();
    }

    IEnumerable<GridPlacementResult> StopPlacing() {
        StopListeningToInput();

        IsPlacing = false;
        _placeTrigger.enabled = false;

        while (_arrows.Count > 0) {
            _arrows.Dequeue().Dispose();
        }
        while (_rangeIndicators.Count > 0) {
            _rangeIndicators.Dequeue().Dispose();
        }

        foreach (var blueprint in _placingObjects.Select(obj => obj.Blueprint.gameObject)) {
            LeanTween.cancel(blueprint);
            Destroy(blueprint);
        }
        
        switch (_state) {
            case State.Deleted:
                return Enumerable.Repeat(GridPlacementResult.Deleted, _placingObjects.Count);
            case State.Placed:
                _cameraShakeEvent.Raise(_placeShake);
                _placingObjects[0].GridObject.PlaceSound.Play();
                return _placingObjects.Select(obj => {
                    var pos = _gridPosition + obj.Offset;
                    _lastRotation = obj.GridRotation;
                    return GridPlacementResult.Successful(pos, obj.GridRotation);
                });
            case State.Waiting:
            case State.Cancelled:
            default:
                return Enumerable.Repeat(GridPlacementResult.Failed, _placingObjects.Count);
        }
    }

    PlacingObject SetupObject(GridObject gridObject, bool keepRotation) {
        var result = new PlacingObject {
            GridObject = gridObject,
        };
        
        var blueprint = Instantiate(gridObject.BlueprintPrefab).transform;
        blueprint.gameObject.SetLayerRecursively(_ignoreRaycastLayer);
        result.Blueprint = blueprint;
        
        result.MoveTarget = Vector3.zero;
        result.RotateTarget = Quaternion.identity;

        // Create arrows as direction markers
        foreach (Transform child in gridObject.transform) {
            if (!child.CompareTag("DirectionMarker")) continue;
            if (!child.gameObject.activeSelf) continue;
            
            var arrow = _arrowPool.Get();
            
            var t = arrow.transform;
            t.SetParent(blueprint, false);
            t.SetLocalPositionAndRotation(child.localPosition, child.localRotation);
            
            _arrows.Enqueue(arrow);
        }
        
        var rangeProviders = gridObject.GetComponentsInChildren<IRangeProvider>();
        if (rangeProviders.Length > 0) {
            var range = rangeProviders.Max(provider => provider.Range) / 4f;
            var indicator = _rangeIndicatorPool.Get();
            
            var t = indicator.transform;
            t.SetParent(blueprint, false);
            t.localScale = new Vector3(range * 2, 1, range * 2);
            t.localPosition = Vector3.up * 0.1f;
            
            _rangeIndicators.Enqueue(indicator);
        }
        
        var renderers = blueprint.GetComponentsInChildren<Renderer>();
        renderers.ApplyMaterial(_validMaterial);
        _renderers.AddRange(renderers);

        result.GridRotation = keepRotation ? gridObject.Rotation : _lastRotation;
        
        return result;
    }

    async Task WaitForPlacement() {
        _state = State.Waiting;

        while (IsPlacing) {
            if (_state != State.Waiting) {
                if (_state is State.Cancelled or State.Deleted) break;

                if (_state == State.Placed) {
                    if (_gridManager.CheckOverlapping(_gridPosition, _combinedSize, out _)) break;
                    _state = State.Waiting;
                }
            }
            await Task.Yield();
        }
    }

    void OnDelete() {
        if (!IsPlacing) return;
        _state = State.Deleted;
    }

    void OnRotate() {
        if (!IsPlacing) return;
        _combinedSize = _combinedSize.Rotated(1);
        foreach (var obj in _placingObjects) {
            obj.GridRotation += 1;
            var newOffset = obj.Offset.RotateAbs(1);
            obj.Offset = newOffset;
        }
        UpdateBlueprints(true);
    }

    void UpdateBlueprints(bool tween) {
        MoveBlueprints(tween);
        RotateBlueprints(tween);
    }

    void RotateBlueprints(bool tween) {
        foreach (var obj in _placingObjects) {
            var newRot = obj.GridRotation.ToQuaternion(Vector3.up);
        
            if (obj.RotateTarget == newRot) return;
            obj.RotateTarget = newRot;
            
            var blueprint = obj.Blueprint;
            if (tween) {
                LeanTween.cancel(blueprint.gameObject);
                LeanTween.rotate(blueprint.gameObject, newRot.eulerAngles, _tweenTime).setEase(_tweenType);
            } else {
                blueprint.rotation = newRot;
            }
        }
    }

    void MoveBlueprints(bool tween) {
        foreach (var obj in _placingObjects) {
            var newTarget = _gridManager.GridToWorld(_gridPosition + obj.Offset, obj.GridObject.StaticSize, obj.GridRotation);
            var currentPos = obj.Blueprint.position;

            if (obj.MoveTarget != newTarget) {
                obj.MoveTarget = newTarget;
                UpdateValidity();
            }
        
            if (currentPos == obj.MoveTarget) return;
            obj.Blueprint.position = tween ? 
                Vector3.SmoothDamp(currentPos, obj.MoveTarget, ref obj.MoveVelocity, _moveTime)
                : obj.MoveTarget;
        }
    }

    void UpdateValidity() {
        var valid = _gridManager.CheckOverlapping(_gridPosition, _combinedSize, out _);
        if (valid == _materialValidity) return;
        
        _materialValidity = valid;
        _renderers.ApplyMaterial(valid ? _validMaterial : _invalidMaterial);
            
        if (_isHolding && valid) {
            _state = State.Placed;
        }
    }

    Vector3 GetMouseWorldPos() {
        var ray = MouseHelpers.ScreenRay;
        return _plane.Raycast(ray, out var enter) ? ray.GetPoint(enter) : Vector3.zero;
    }
    
    void UpdateGridPosition() {
        var newPosition = _gridManager.WorldToGrid(GetMouseWorldPos()) - _combinedSize.Bounds / 2;
        if (newPosition == _gridPosition) return;
        
        if (Keyboard.current.shiftKey.isPressed) {
            if (_moveAxis == Vector2Int.right) {
                newPosition.y = _gridPosition.y;
            } else if (_moveAxis == Vector2Int.up) {
                newPosition.x = _gridPosition.x;
            }
        } else {
            var delta = newPosition - _gridPosition;
            delta.x = Mathf.Abs(delta.x);
            delta.y = Mathf.Abs(delta.y);
            
            if (delta.x > delta.y) {
                _moveAxis = Vector2Int.right;
            } else if (delta.x < delta.y) {
                _moveAxis = Vector2Int.up;
            }
        }

        _gridPosition = newPosition;
    }
    
    public void OnPointerDown(PointerEventData eventData) {
        if (!_allowedButtons.Contains(eventData.button)) return;
        _isHolding = true;
        
        if (IsPlacing) _state = State.Placed;
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (!_allowedButtons.Contains(eventData.button)) return;
        _isHolding = false;
    }

    void OnDrawGizmos() {
        if (!IsPlacing) return;

        Gizmos.color = Color.red;
        foreach (var position in _combinedSize.GetBlockingPositions()) {
            Gizmos.DrawWireCube(_gridManager.GridToWorld(_gridPosition + position), GridObject.PreviewCellSize);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_gridManager.GridToWorld(_gridPosition), GridObject.PreviewCellSize);
    }

    enum State {
        Waiting,
        Placed,
        Cancelled,
        Deleted
    }

    class PlacingObject {
        public GridObject GridObject;
        public Transform Blueprint;
        public Vector2Int Offset;
        public GridRotation GridRotation;
        public bool MaterialValidity;
        
        public Vector3 MoveTarget;
        public Vector3 MoveVelocity;
        public Quaternion RotateTarget;
    }
}

}

public readonly struct GridPlacementResult {
    public readonly Vector2Int GridPosition;
    public readonly GridRotation Rotation;
    public readonly GridPlacementResultType ResultType;
    public bool WasSuccessful => ResultType == GridPlacementResultType.Successful;

    GridPlacementResult(GridPlacementResultType resultType, Vector2Int gridPosition = default, GridRotation rotation = default) {
        GridPosition = gridPosition;
        Rotation = rotation;
        ResultType = resultType;
    }
    
    public static implicit operator bool(GridPlacementResult result) => result.WasSuccessful;
    
    public static GridPlacementResult Failed => new(GridPlacementResultType.Failed);
    public static GridPlacementResult Deleted => new(GridPlacementResultType.Deleted);
    public static GridPlacementResult Successful(Vector2Int gridPosition, GridRotation rotation) => new(GridPlacementResultType.Successful, gridPosition, rotation);
}

public enum GridPlacementResultType {
    Successful,
    Failed,
    Deleted
}