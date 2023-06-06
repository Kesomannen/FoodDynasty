using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GridObjectPlacer : MonoBehaviour, IPointerClickHandler, IPointerMoveHandler {
    [Header("References")]
    [SerializeField] GridManager _gridManager;
    [SerializeField] Collider _placeTrigger;
    
    [Header("Input")]
    [SerializeField] string _rotateActionName = "Rotate";
    [SerializeField] string _cancelActionName = "Cancel";
    [SerializeField] InputActionAsset _inputActionAsset;
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
    
    Renderer[] _renderers;
    bool _currentMaterialIsValid;
    
    bool _hasPlaced;
    bool _hasCancelled;
    
    InputAction _rotateAction;
    InputAction _cancelAction;

    void Awake() {
        _placeTrigger.enabled = false;
        
        _rotateAction = _inputActionAsset.FindAction(_rotateActionName);
        _cancelAction = _inputActionAsset.FindAction(_cancelActionName);
    }

    void OnEnable() {
        _rotateAction.Enable();
        _cancelAction.Enable();
        
        _rotateAction.performed += OnRotate;
        _cancelAction.performed += OnCancel;
    }

    void OnDisable() {
        _rotateAction.Disable();
        _cancelAction.Disable();
        
        _rotateAction.performed -= OnRotate;
        _cancelAction.performed -= OnCancel;
    }
    
    public void Cancel() {
        if (!IsPlacing) return;
        _hasCancelled = true;
    }

    public async Task<PlacementResult> DoPlacement(IGridObject dataObject, bool keepRotation = true, bool keepPosition = true) {
        if (IsPlacing) {
            return new PlacementResult { Succeeded = false };
        }
        
        _currentObject = dataObject;
        if (keepRotation) {
            _currentRotation = dataObject.Rotation;
        }
        if (keepPosition) {
            _currentPosition = dataObject.GridPosition;
        }

        IsPlacing = true;
        _placeTrigger.enabled = true;
        SetupBlueprint(dataObject);
        
        await WaitForPlacement(dataObject);

        IsPlacing = false;
        _placeTrigger.enabled = false;
        Destroy(_currentBlueprint.gameObject);
        
        if (_hasCancelled) {
            return new PlacementResult { Succeeded = false };
        }
        
        return new PlacementResult {
            GridPosition = _currentPosition,
            Rotation = _currentRotation,
            Succeeded = true
        };
    }

    void SetupBlueprint(IGridObject gridObject) {
        _currentBlueprint = Instantiate(gridObject.BlueprintPrefab).transform;
        
        _renderers = _currentBlueprint.GetComponentsInChildren<Renderer>();
        _renderers.ApplyMaterial(_validMaterial);
        
        _currentMaterialIsValid = true;
        UpdateBlueprint();
    }

    async Task WaitForPlacement(IGridObject gridObject) {
        _hasPlaced = false;
        _hasCancelled = false;

        while (true) {
            if (_hasPlaced) {
                if (_gridManager.CanAdd(gridObject, _currentPosition, _currentRotation)) {
                    break;
                }

                _hasPlaced = false;
            }

            if (_hasCancelled) {
                break;
            }

            await Task.Yield();
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (!IsPlacing) return;
        if (!_allowedButtons.Contains(eventData.button)) return;
        _hasPlaced = true;
    }

    public void OnPointerMove(PointerEventData eventData) {
        if (!IsPlacing) return;
        _currentPosition = GetGridPos(eventData);
        UpdateBlueprint();
    }
    
    void OnCancel(InputAction.CallbackContext context) {
        if (!IsPlacing) return;
        _hasCancelled = true;
    }

    void OnRotate(InputAction.CallbackContext context) {
        if (!IsPlacing) return;
        _currentRotation += 1;
        UpdateBlueprint();
    }

    void UpdateBlueprint() {
        _currentBlueprint.rotation = _currentRotation.ToQuaternion(Vector3.up);
        
        var newPos = _gridManager.GridToWorld(_currentPosition, _currentObject.StaticSize, _currentRotation);
        if (_currentBlueprint.position != newPos) {
            UpdateBlueprintMaterial();
        }
        
        _currentBlueprint.position = newPos;
    }

    void UpdateBlueprintMaterial() {
        var valid = _gridManager.CanAdd(_currentObject, _currentPosition, _currentRotation);
        if (valid == _currentMaterialIsValid) return;

        _currentMaterialIsValid = valid;
        _renderers.ApplyMaterial(valid ? _validMaterial : _invalidMaterial);
    }

    static Vector3 GetWorldPos(PointerEventData eventData) {
        return eventData.pointerCurrentRaycast.worldPosition;
    }
    
    Vector2Int GetGridPos(PointerEventData eventData) {
        return _gridManager.WorldToGrid(GetWorldPos(eventData));
    }

    public struct PlacementResult {
        public Vector2Int GridPosition;
        public GridRotation Rotation;
        public bool Succeeded;
    }
}