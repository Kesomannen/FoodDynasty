using UnityEngine;

public class IdleRotation : MonoBehaviour {
    [SerializeField] Vector3 _rotation;
    
    Transform _transform;
    
    void Awake() {
        _transform = transform;
    }
    
    void Update() {
        _transform.Rotate(_rotation * Time.deltaTime);
    }
}