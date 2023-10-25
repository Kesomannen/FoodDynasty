using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dynasty.Core.Grid;
using Dynasty.Core.Tooltip;
using Dynasty.Library;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Dynasty.Core.Grid {

/// <summary>
/// Handles the placement of <see cref="GridObject"/>s on a <see cref="GridManager"/> using player input.
/// </summary>
public class GridObjectPlacer : MonoBehaviour, IPointerClickHandler {
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
    
    [Foldout("Events")]
    [Tooltip("Raised when an object is placed.")]
    [SerializeField] UnityEvent _onPlaced;
    
    [Foldout("Events")]
    [Tooltip("Raised when placement is started.")]
    [SerializeField] UnityEvent _onStartPlacing;

    [Foldout("Events")]
    [Tooltip("Raised when placement is ended.")]
    [SerializeField] UnityEvent _onStopPlacing;
    
    /// <summary>
    /// Is the placer currently placing an object?
    /// </summary>
    public bool IsPlacing { get; private set; }
    public GridManager GridManager => _gridManager;

    readonly Queue<Poolable> _arrows = new();
    Poolable _rangeIndicator;
    
    GridObject _gridObject;
    Transform _blueprint;
    Vector2Int _gridPosition;
    GridRotation _gridRotation;

    Renderer[] _renderers;
    bool _materialValidity;
    
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
        
        _gridPosition = GetMouseGridPos();
        UpdateBlueprint(true);
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

        _gridObject = data;
        if (keepRotation) _gridRotation = data.Rotation;
        _gridPosition = keepPosition ? data.GridPosition : GetMouseGridPos();

        IsPlacing = true;
        _placeTrigger.enabled = true;
        SetupBlueprint(data);
        
        _onStartPlacing.Invoke();
        
        StartListeningToInput();
        await WaitForPlacement(data);
        
        return IsPlacing ? StopPlacing() : GridPlacementResult.Failed;
    }

    GridPlacementResult StopPlacing() {
        StopListeningToInput();

        IsPlacing = false;
        _placeTrigger.enabled = false;

        while (_arrows.Count > 0) {
            _arrows.Dequeue().Dispose();
        }

        if (_rangeIndicator != null) {
            _rangeIndicator.Dispose();
            _rangeIndicator = null;
        }

        LeanTween.cancel(_rotateTweenId);
        Destroy(_blueprint.gameObject);
        
        _onStopPlacing.Invoke();

        switch (_state) {
            case State.Deleted:
                return GridPlacementResult.Deleted;
            case State.Placed:
                _onPlaced.Invoke();
                return GridPlacementResult.Successful(_gridPosition, _gridRotation);
            default:
                return GridPlacementResult.Failed;
        }
    }

    void SetupBlueprint(GridObject gridObject) {
        _blueprint = Instantiate(gridObject.BlueprintPrefab).transform;
        
        _moveTarget = Vector3.zero;
        _rotateTarget = Quaternion.identity;

        // Create arrows as direction markers
        foreach (Transform child in gridObject.transform) {
            if (!child.CompareTag("DirectionMarker")) continue;
            if (!child.gameObject.activeSelf) continue;
            
            var arrow = _arrowPool.Get();
            
            var t = arrow.transform;
            t.SetParent(_blueprint, false);
            t.SetLocalPositionAndRotation(child.localPosition, child.localRotation);
            
            _arrows.Enqueue(arrow);
        }
        
        var rangeProviders = gridObject.GetComponentsInChildren<IRangeProvider>();
        if (rangeProviders.Length > 0) {
            var range = rangeProviders.Max(provider => provider.Range) / 4f;
            _rangeIndicator = _rangeIndicatorPool.Get();
            
            var t = _rangeIndicator.transform;
            t.SetParent(_blueprint, false);
            t.localScale = new Vector3(range * 2, 1, range * 2);
            t.localPosition = Vector3.zero;
        }
        
        _renderers = _blueprint.GetComponentsInChildren<Renderer>();
        _renderers.ApplyMaterial(_validMaterial);
        
        _materialValidity = true;
        UpdateBlueprint(false);
    }

    async Task WaitForPlacement(GridObject gridObject) {
        _state = State.Waiting;

        while (IsPlacing) {
            if (_state != State.Waiting) {
                if (_state is State.Cancelled or State.Deleted) break;

                if (_state == State.Placed) {
                    if (_gridManager.CanAdd(gridObject, _gridPosition, _gridRotation)) break;
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
        _gridRotation += 1;
        UpdateBlueprint(true);
    }

    void UpdateBlueprint(bool tween) {
        MoveBlueprint(tween);
        RotateBlueprint(tween);
    }

    void RotateBlueprint(bool tween) {
        var newRot = _gridRotation.ToQuaternion(Vector3.up);
        
        if (_rotateTarget == newRot) return;
        _rotateTarget = newRot;

        if (tween) {
            LeanTween.cancel(_rotateTweenId);
            _rotateTweenId = LeanTween
                .rotate(_blueprint.gameObject, newRot.eulerAngles, _tweenTime)
                .setEase(_tweenType)
                .uniqueId;
        } else {
            _blueprint.rotation = newRot;
        }
    }

    void MoveBlueprint(bool tween) {
        var newTarget = _gridManager.GridToWorld(_gridPosition, _gridObject.StaticSize, _gridRotation);
        var currentPos = _blueprint.position;

        if (_moveTarget != newTarget) {
            _moveTarget = newTarget;
            UpdateBlueprintValidity();
        }
        
        if (currentPos == _moveTarget) return;
        _blueprint.position = tween ? 
            Vector3.SmoothDamp(currentPos, _moveTarget, ref _moveVelocity, _moveTime)
            : _moveTarget;
    }

    void UpdateBlueprintValidity() {
        var valid = _gridManager.CanAdd(_gridObject, _gridPosition, _gridRotation);
        if (valid == _materialValidity) return;

        _materialValidity = valid;
        _renderers.ApplyMaterial(valid ? _validMaterial : _invalidMaterial);
    }

    Vector3 GetMouseWorldPos() {
        var ray = MouseHelpers.ScreenRay;
        return _plane.Raycast(ray, out var enter) ? ray.GetPoint(enter) : Vector3.zero;
    }
    
    Vector2Int GetMouseGridPos() {
        var currentBounds = _gridObject.StaticSize.Rotated(_gridRotation.Steps).Bounds;
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