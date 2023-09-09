using UnityEngine;

namespace Dynasty.UI.Menu {

public class RotatingCamera : MonoBehaviour {
    [SerializeField] float _rotateSpeed;
    
    void LateUpdate() {
        transform.Rotate(Vector3.up, _rotateSpeed * Time.deltaTime);
    }
}

}