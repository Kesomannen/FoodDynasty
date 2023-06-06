using UnityEngine;

public static class VectorExtensions {
    public static Vector2 RotateCCW(this Vector2 vector) {
        return new Vector2(-vector.y, vector.x);
    }
    
    public static Vector2Int ToVector2Int(this Vector2 vector) {
        return new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
    }
}