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
    [SerializeField] string _moveActionName = "Camera/Move";
    [SerializeField] string _zoomActionName = "Camera/Zoom";
    [SerializeField] InputActionAsset _inputAsset;
    
    [Header("References")]
    [SerializeField] Transform _cameraTransform;

    Transform _transform;
    
    Vector3 _movementVelocity;
    Vector3 _targetPos;
    
    Vector3 _targetZoom;
    Vector3 _zoomVelocity;

    InputAction _moveAction;
    InputAction _zoomAction;

    void Awake() {
        _transform = transform;
        _targetPos = _transform.position;
        _targetZoom = _cameraTransform.localPosition;
        
        _moveAction = _inputAsset.FindAction(_moveActionName);
        _zoomAction = _inputAsset.FindAction(_zoomActionName);
    }
    
    void OnEnable() {
        _moveAction.Enable();
        _zoomAction.Enable();
    }
    
    void OnDisable() {
        _moveAction.Disable();
        _zoomAction.Disable();
    }

    void LateUpdate() {
        HandleMovement();
        HandleZoom();
    }

    void HandleMovement() {
        var value = _moveAction.ReadValue<Vector2>();
        var direction = _transform.forward * value.y + _transform.right * value.x;
        _targetPos += direction * _moveSpeed * Time.deltaTime;

        var newPos = Vector3.SmoothDamp(_transform.position, _targetPos, ref _movementVelocity, _smoothTime);
        _transform.position = newPos;
    }
    
    void HandleZoom() {
        var value = Mathf.Clamp(_zoomAction.ReadValue<float>(), -1, 1);
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