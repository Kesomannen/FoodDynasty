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
    [SerializeField] float _smoothTime;

    [Header("Input")] 
    [SerializeField] InputActionReference _moveAction;
    [SerializeField] InputActionReference _zoomAction;
    [SerializeField] InputActionReference _rotateAction;

    [Header("References")]
    [SerializeField] Transform _cameraTransform;

    Transform _transform;
    
    Vector3 _movementVelocity;
    Vector3 _targetPos;
    
    Vector3 _targetZoom;
    Vector3 _zoomVelocity;
    
    float _rotateVelocity;
    float _targetRotation;

    void Awake() {
        _transform = transform;
        _targetPos = _transform.position;
        _targetZoom = _cameraTransform.localPosition;
    }
    
    void OnEnable() {
        _moveAction.action.Enable();
        _zoomAction.action.Enable();
        _rotateAction.action.Enable();
    }
    
    void OnDisable() {
        _moveAction.action.Disable();
        _zoomAction.action.Disable();
        _rotateAction.action.Disable();
    }

    void LateUpdate() {
        HandleMovement();
        HandleZoom();
    }

    void HandleMovement() {
        var value = _moveAction.action.ReadValue<Vector2>();
        var direction = _transform.forward * value.y + _transform.right * value.x;
        _targetPos += direction * _moveSpeed * Time.deltaTime;

        var newPos = Vector3.SmoothDamp(_transform.position, _targetPos, ref _movementVelocity, _smoothTime);
        _transform.position = newPos;
    }
    
    void HandleZoom() {
        var value = Mathf.Clamp(_zoomAction.action.ReadValue<float>(), -1, 1);
        _targetZoom += value * _zoomDirection * _zoomSpeed * Time.deltaTime;
        
        ClampZoom();

        var newPos = Vector3.SmoothDamp(_cameraTransform.localPosition, _targetZoom, ref _zoomVelocity, _smoothTime);
        _cameraTransform.localPosition = newPos;
    }

    void ClampZoom() {
        var zoomLevel = _targetZoom.magnitude;
        if (zoomLevel < _zoomRange.x) {
            _targetZoom = _targetZoom.normalized * _zoomRange.x;
        } else if (zoomLevel > _zoomRange.y) {
            _targetZoom = _targetZoom.normalized * _zoomRange.y;
        }
    }
    
    void HandleRotation() {
        var value = Mathf.Clamp(_rotateAction.action.ReadValue<float>(), -1, 1);
        _targetRotation += value * _rotateSpeed * Time.deltaTime;

        if (Mathf.Approximately(_rotateVelocity, 0)) return;
        var newRotation = Mathf.SmoothDampAngle(_transform.eulerAngles.y, _targetRotation, ref _rotateVelocity, _smoothTime);
        _transform.eulerAngles = new Vector3(0, newRotation, 0);
    }
}