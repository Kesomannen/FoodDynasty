using UnityEngine;

public class KeepRotation : MonoBehaviour {
    Quaternion _rotation;
    Transform _transform;
    
    void Awake() {
        _transform = transform;
        _rotation = _transform.rotation;
    }

    void LateUpdate() {
        transform.rotation = _rotation;
    }
}