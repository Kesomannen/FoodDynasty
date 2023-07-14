using UnityEngine;

namespace Dynasty.Library.Extensions {

public static class VectorExtensions {
    public static Vector2 RotateCCW(this Vector2 vector) {
        return new Vector2(-vector.y, vector.x);
    }
    
    public static Vector2Int RotateCCW(this Vector2Int vector) {
        return new Vector2Int(-vector.y, vector.x);
    }
    
    public static Vector2Int RotateAbs(this Vector2Int vector, int steps) {
        return steps % 2 == 0 ? vector : new Vector2Int(vector.y, vector.x);
    }
    
    public static Vector2Int ToVector2Int(this Vector2 vector) {
        return new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
    }
    
    public static bool InRange(this Vector2Int range, int value, bool inclusive = true) {
        return inclusive ?
            value >= range.x && value <= range.y 
            : value > range.x && value < range.y;
    }
}

}