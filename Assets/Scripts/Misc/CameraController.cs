using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] Vector2 _zoomRange;
    [SerializeField] Vector3 _zoomDirection;
    [SerializeField] float _zoomSpeed;
    [Space]
    [SerializeField] float _moveSpeed;
    [SerializeField] float _rotateSpeed;
    [Space]
    [FormerlySerializedAs("_moveDragSpeed")]
    [SerializeField] float _panSpeed;
    [FormerlySerializedAs("_rotateDragSpeed")]
    [SerializeField] float _spinSpeed;
    [Space]
    [SerializeField] float _smoothTime;

    [Header("Input")]
    [SerializeField] InputActionReference _moveAction;
    [SerializeField] InputActionReference _zoomAction;
    [SerializeField] InputActionReference _rotateAction;
    
    [Header("References")] 
    [SerializeField] Camera _camera;
        
    Transform _cameraTransform;
    Transform _transform;
    
    Vector3 _movementVelocity;
    Vector3 _targetPos;
    
    Vector3 _zoomVelocity;
    Vector3 _targetZoom;

    float _rotationVelocity;
    float _targetRotation;
    
    Plane _raycastPlane;
    Vector3 _panDragPos;
    bool _isPanning;
    bool _isSpinning;

    void Awake() {
        _transform = transform;
        _cameraTransform = _camera.transform;
        
        _targetPos = _transform.position;
        _targetZoom = _cameraTransform.localPosition;
        _targetRotation = _transform.eulerAngles.y;
        
        _raycastPlane = new Plane(Vector3.up, Vector3.zero);
    }
    
    void LateUpdate() {
        GetMovementInput();
        GetZoomInput();
        GetRotationInput();
        
        HandleMovement();
        HandleZoom();
        HandleRotation();
    }

    void HandleMovement() {
        var newPos = Vector3.SmoothDamp(_transform.position, _targetPos, ref _movementVelocity, _smoothTime);
        _transform.position = newPos;
    }

    void HandleZoom() {
        var newZoom = Vector3.SmoothDamp(_cameraTransform.localPosition, _targetZoom, ref _zoomVelocity, _smoothTime);
        _cameraTransform.localPosition = newZoom;
    }

    void HandleRotation() {
        var newRotation = Mathf.SmoothDampAngle(_transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, _smoothTime);
        _transform.eulerAngles = new Vector3(0, newRotation, 0);
    }

    void GetMovementInput() {
        var value = _moveAction.action.ReadValue<Vector2>();
        var direction = _transform.forward * value.y + _transform.right * value.x;
        _targetPos += direction * _moveSpeed * Time.deltaTime;
    }

    void GetZoomInput() {
        var value = Mathf.Clamp(_zoomAction.action.ReadValue<float>(), -1, 1);
        _targetZoom += value * _zoomDirection * _zoomSpeed * Time.deltaTime;
        ClampZoom();
    }
    
    void GetRotationInput() {
        var value = Mathf.Clamp(_rotateAction.action.ReadValue<float>(), -1, 1);
        _targetRotation += value * _rotateSpeed * Time.deltaTime;
    }

    public void OnMouseMove(InputAction.CallbackContext context) {
        var mousePosition = Mouse.current.position.ReadValue();
        var mouseDelta = context.ReadValue<Vector2>();
        
        if (_isPanning) {
            var currentRay = _camera.ScreenPointToRay(mousePosition);
            if (!_raycastPlane.Raycast(currentRay, out var entry)) return;
            
            var currentPos = currentRay.GetPoint(entry);
            _targetPos += (_panDragPos - currentPos) * _panSpeed;
            
            _panDragPos = currentPos;
        } else if (_isSpinning) {
            _targetRotation += mouseDelta.x * _spinSpeed;
        }
    }
    
    public void OnPan(InputAction.CallbackContext context) {
        if (context.started) {
            var mousePosition = Mouse.current.position.ReadValue();
            var startRay = _camera.ScreenPointToRay(mousePosition);
            
            if (_raycastPlane.Raycast(startRay, out var entry)) {
                _panDragPos = startRay.GetPoint(entry);
            }
        }
        
        _isPanning = context.started || context.performed;
    }
    
    public void OnSpin(InputAction.CallbackContext context) {
        _isSpinning = context.started || context.performed;
    }

    void ClampZoom() {
        var zoomLevel = _targetZoom.magnitude;

        if (zoomLevel < _zoomRange.x) {
            _targetZoom = _zoomDirection.normalized * _zoomRange.x;
            _zoomVelocity = Vector3.zero;
        } else if (zoomLevel > _zoomRange.y) {
            _targetZoom = _zoomDirection.normalized * _zoomRange.y;
            _zoomVelocity = Vector3.zero;
        }
    }
}