using System;
using UnityEngine;

namespace Dynasty.UI.Misc {

public class WorldCanvasRoot : MonoBehaviour {
    [SerializeField] Transform _cameraRig;

    Transform _transform;

    void Awake() {
        _transform = transform;
    }

    void Update() {
        _transform.rotation = _cameraRig.rotation;
    }
}

}