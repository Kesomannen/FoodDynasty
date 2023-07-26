using UnityEngine;

namespace Misc {

public class Floater : MonoBehaviour {
    [SerializeField] float _moveIntensity = 1f;
    [SerializeField] float _moveSpeed = 1f;
    [Space]
    [SerializeField] float _rotationIntensity = 1f;
    [SerializeField] float _rotationSpeed = 1f;

    Quaternion _startRotation;
    Vector3 _startPosition;
    Transform _transform;

    void Awake() {
        _transform = transform;
        _startPosition = _transform.position;
        _startRotation = _transform.rotation;
    }

    void Update() {
        var position = _startPosition + Vector3.up * Mathf.Sin(Time.time * _moveSpeed) * _moveIntensity;
        
        var xRotation = Mathf.Sin(Time.time * _rotationSpeed) * _rotationIntensity;
        var yRotation = Mathf.Cos(Time.time * _rotationSpeed) * _rotationIntensity;
        var zRotation = Mathf.Sin(Time.time * _rotationSpeed) * _rotationIntensity;
        
        var rotation = _startRotation * Quaternion.Euler(xRotation, yRotation, zRotation);
        
        _transform.SetPositionAndRotation(position, rotation);
    }
}

}