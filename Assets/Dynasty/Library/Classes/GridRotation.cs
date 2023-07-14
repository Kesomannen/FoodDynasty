using System;
using Dynasty.Library.Extensions;
using UnityEngine;

namespace Dynasty.Library.Classes {

/// <summary>
/// Represents a 2d rotation in 90 degree steps.
/// </summary>
[Serializable]
public struct GridRotation {
    [SerializeField] int _value;

    public int Steps => _value;
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

    public GridRotation(Quaternion rotation, Vector3 axis) {
        var forward = rotation * Vector3.forward;
        var angle = Mathf.Atan2(Vector3.Cross(forward, axis).magnitude, Vector3.Dot(forward, axis)) * Mathf.Rad2Deg;
        _value = Mathf.RoundToInt(angle / 90f);
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

    public Vector2Int Rotate(Vector2Int vector) {
        for (var i = 0; i < _value; i++) {
            vector = vector.RotateCCW();
        }

        return vector;
    }

    public static GridRotation operator +(GridRotation rotation, int value) {
        return rotation.Rotated(value);
    }

    public override string ToString() {
        return $"{Degrees}°";
    }
}

}