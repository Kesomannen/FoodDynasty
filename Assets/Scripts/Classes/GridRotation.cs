using System;
using UnityEngine;

[Serializable]
public struct GridRotation {
    [SerializeField] int _value;
    
    public int Degrees => _value * 90;
    public Vector2Int Vector => _value switch {
        0 => Vector2Int.up,
        1 => Vector2Int.right,
        2 => Vector2Int.down,
        3 => Vector2Int.left,
        _ => Vector2Int.zero
    };
    
    public GridRotation(int value) {
        _value = Clamp(value);
    }

    static int Clamp(int value) {
        if (value < 0) {
            return 4 + value % 4;
        }

        return value % 4;
    }
    
    public GridRotation Rotated(int steps) {
        return new GridRotation(_value + steps);
    }
    
    public Quaternion ToQuaternion(Vector3 axis) {
        return Quaternion.AngleAxis(Degrees, axis);
    }
    
    public Vector2 Rotate(Vector2 vector) {
        for (var i = 0; i < _value; i++) {
            vector = vector.RotateCCW();
        }
        return vector;
    }
    
    public Vector2Int RotateSize(Vector2Int size) {
        return _value % 2 == 0 ? size : new Vector2Int(size.y, size.x);
    }

    public static GridRotation operator +(GridRotation rotation, int value) {
        return rotation.Rotated(value);
    }
}