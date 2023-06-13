using UnityEngine;

public static class CanvasHelpers {
    public static Canvas Canvas {
        get {
            if (_canvas != null) return _canvas;
            _canvas = Object.FindObjectOfType<Canvas>();
            return _canvas;
        }
    }
    
    static Canvas _canvas;
    
    public static Vector2 CanvasToScreen(Vector2 canvasPosition) {
        return canvasPosition / Canvas.transform.localScale;
    }
    
    public static Vector2 CanvasToScreen(float x, float y) {
        return CanvasToScreen(new Vector2(x, y));
    }
    
    public static Vector2 ScreenToCanvas(Vector2 screenPosition) {
        return screenPosition * Canvas.transform.localScale;
    }
    
    public static Vector2 ScreenToCanvas(float x, float y) {
        return ScreenToCanvas(new Vector2(x, y));
    }
}