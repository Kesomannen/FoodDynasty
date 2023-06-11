using UnityEngine;
using UnityEngine.InputSystem;

public static class MouseHelpers {
    static Camera _mainCam;

    static Camera MainCamera {
        get {
            if (_mainCam == null) {
                _mainCam = Camera.main;
            }
            return _mainCam;
        }
    }
    
    public static Vector2 MouseScreenPosition => Mouse.current.position.ReadValue();
    public static Vector2 MouseWorldPosition => MainCamera.ScreenToWorldPoint(MouseScreenPosition);
}