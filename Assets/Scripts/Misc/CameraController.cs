using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] Vector2 _zoomRange;
    [SerializeField] Vector3 _zoomDirection;
    [SerializeField] float _zoomSpeed;
    [SerializeField] float _moveSpeed;
    [SerializeField] float _smoothTime;

    [Header("Input")] 
    [SerializeField] InputActionReference _moveAction;
    [SerializeField] InputActionReference _zoomAction;

    [Header("References")]
    [SerializeField] Transform _cameraTransform;

    Transform _transform;
    
    [Header("Debug")]
    [ReadOnly] [AllowNesting]
    [SerializeField] Vector3 _movementVelocity;
    [ReadOnly] [AllowNesting]
    [SerializeField] Vector3 _targetPos;
    [ReadOnly] [AllowNesting]
    [SerializeField] Vector3 _targetZoom;
    [ReadOnly] [AllowNesting]
    [SerializeField] Vector3 _zoomVelocity;

    void Awake() {
        _transform = transform;
        _targetPos = _transform.position;
        _targetZoom = _cameraTransform.localPosition;
    }
    
    void OnEnable() {
        _moveAction.action.Enable();
        _zoomAction.action.Enable();
    }
    
    void OnDisable() {
        _moveAction.action.Disable();
        _zoomAction.action.Disable();
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
}