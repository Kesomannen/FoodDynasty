using UnityEngine;
using UnityEngine.InputSystem;

namespace Dynasty.Library.Helpers {

public static class MouseHelpers {
    static Camera _mainCam;

    public static Camera MainCamera {
        get {
            if (_mainCam == null) {
                _mainCam = Camera.main;
            }
            return _mainCam;
        }
    }
    
    public static Vector2 ScreenPosition => Mouse.current.position.ReadValue();
    public static Vector2 WorldPosition => MainCamera.ScreenToWorldPoint(ScreenPosition);
    public static Ray ScreenRay => MainCamera.ScreenPointToRay(ScreenPosition);
}

}