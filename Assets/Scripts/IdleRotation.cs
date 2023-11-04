using UnityEngine;

public class IdleRotation : MonoBehaviour {
    [SerializeField] Vector3 _rotation;
    [SerializeField] Vector3 _offset;
    
    Transform _transform;
    
    void Awake() {
        _transform = transform;
        _transform.Rotate(_offset);
    }
    
    void Update() {
        _transform.Rotate(_rotation * Time.deltaTime);
    }
}