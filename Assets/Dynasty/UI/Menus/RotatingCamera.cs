using UnityEngine;

namespace Dynasty.UI {

public class RotatingCamera : MonoBehaviour {
    [SerializeField] float _rotateSpeed;
    
    void LateUpdate() {
        transform.Rotate(Vector3.up, _rotateSpeed * Time.deltaTime);
    }
}

}