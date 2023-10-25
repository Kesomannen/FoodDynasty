using System;
using Dynasty.Library;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour {
    [Header("Movement")]
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
    [SerializeField] Bounds _bounds;

    [Header("Input")]
    [SerializeField] InputActionReference _moveAction;
    [SerializeField] InputActionReference _zoomAction;
    [SerializeField] InputActionReference _rotateAction;
    [Space]
    [SerializeField] InputActionReference _panAction;
    [SerializeField] InputActionReference _spinAction;
    [SerializeField] InputActionReference _moveMouseAction;
    
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

    static float Sensitivity => Settings.CameraSensitivity.Value;

    void Awake() {
        _transform = transform;
        _cameraTransform = _camera.transform;
        
        _targetPos = _transform.position;
        _targetZoom = _cameraTransform.localPosition;
        _targetRotation = _transform.eulerAngles.y;
        
        _raycastPlane = new Plane(Vector3.up, Vector3.zero);
    }

    void OnEnable() {
        _moveAction.action.actionMap.Enable();

        _panAction.action.started += OnPanStarted;
        _spinAction.action.started += OnSpinStarted;
        
        _panAction.action.canceled += OnPanEnded;
        _spinAction.action.canceled += OnSpinEnded;
        
        _moveMouseAction.action.performed += OnMouseMoved;
    }
    
    void OnDisable() {
        _moveAction.action.actionMap.Disable();
        
        _panAction.action.started -= OnPanStarted;
        _spinAction.action.started -= OnSpinStarted;
        
        _panAction.action.canceled -= OnPanEnded;
        _spinAction.action.canceled -= OnSpinEnded;
        
        _moveMouseAction.action.performed -= OnMouseMoved;
    }

    void LateUpdate() {
        GetMovementInput();
        GetZoomInput();
        GetRotationInput();
        
        HandleMovement();
        HandleZoom();
        HandleRotation();
        
        ClampPosition();
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
    
    void ClampPosition() {
        var pos = _transform.position;
        pos.x = Mathf.Clamp(pos.x, _bounds.min.x, _bounds.max.x);
        pos.z = Mathf.Clamp(pos.z, _bounds.min.z, _bounds.max.z);
        _transform.position = pos;
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

    void OnMouseMoved(InputAction.CallbackContext context) {
        var mousePosition = Mouse.current.position.ReadValue();
        var mouseDelta = context.ReadValue<Vector2>();
        
        if (_isPanning) {
            var currentRay = _camera.ScreenPointToRay(mousePosition);
            if (!_raycastPlane.Raycast(currentRay, out var entry)) return;
            
            var currentPos = currentRay.GetPoint(entry);
            _targetPos += (_panDragPos - currentPos) * _panSpeed * Sensitivity;
            
            _panDragPos = currentPos;
        } else if (_isSpinning) {
            _targetRotation += mouseDelta.x * _spinSpeed * Sensitivity;
        }
    }

    void OnPanStarted(InputAction.CallbackContext context) {
        var mousePosition = Mouse.current.position.ReadValue();
        var startRay = _camera.ScreenPointToRay(mousePosition);
            
        if (_raycastPlane.Raycast(startRay, out var entry)) {
            _panDragPos = startRay.GetPoint(entry);
        }

        _isPanning = true;
    }

    void OnSpinStarted(InputAction.CallbackContext context) {
        _isSpinning = true;
    }
    
    void OnPanEnded(InputAction.CallbackContext context) {
        _isPanning = false;
    }
    
    void OnSpinEnded(InputAction.CallbackContext context) {
        _isSpinning = false;
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

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_bounds.center, _bounds.size);
    }
}