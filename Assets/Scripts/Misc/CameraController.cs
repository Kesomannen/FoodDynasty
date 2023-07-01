using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] Vector2 _zoomRange;
    [SerializeField] Vector3 _zoomDirection;
    [SerializeField] float _zoomSpeed;
    [Space]
    [SerializeField] float _moveSpeed;
    [SerializeField] float _rotateSpeed;
    [Space]
    [SerializeField] float _moveDragSpeed;
    [SerializeField] float _rotateDragSpeed;
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
    
    Plane _dragPlane;
    Vector3 _moveDragStartPos;
    bool _isMoveDragging;
    bool _isRotateDragging;

    void Awake() {
        _transform = transform;
        _cameraTransform = _camera.transform;
        
        _targetPos = _transform.position;
        _targetZoom = _cameraTransform.localPosition;
        _targetRotation = _transform.eulerAngles.y;
        
        _dragPlane = new Plane(Vector3.up, Vector3.zero);
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
        
        if (_isMoveDragging) {
            var currentRay = _camera.ScreenPointToRay(mousePosition);

            if (!_dragPlane.Raycast(currentRay, out var entry)) return;
            var offset = _moveDragStartPos - currentRay.GetPoint(entry);
            Debug.Log($"Drag offset {offset}");
            _targetPos += offset * _moveDragSpeed;
        } else if (_isRotateDragging) {
            _targetRotation += mouseDelta.x * _rotateDragSpeed;
        }
    }
    
    public void OnDragMove(InputAction.CallbackContext context) {
        if (context.started) {
            var mousePosition = Mouse.current.position.ReadValue();
            var startRay = _camera.ScreenPointToRay(mousePosition);
            
            if (_dragPlane.Raycast(startRay, out var entry)) {
                _moveDragStartPos = startRay.GetPoint(entry);
                Debug.Log($"Start drag at {_moveDragStartPos}");
            }
        }
        
        _isMoveDragging = context.started || context.performed;
    }
    
    public void OnDragRotate(InputAction.CallbackContext context) {
        _isRotateDragging = context.started || context.performed;
    }

    void ClampZoom() {
        var zoomLevel = _targetZoom.magnitude;
        
        if (zoomLevel < _zoomRange.x) {
            _targetZoom = _targetZoom.normalized * _zoomRange.x;
        } else if (zoomLevel > _zoomRange.y) {
            _targetZoom = _targetZoom.normalized * _zoomRange.y;
        }
    }
}