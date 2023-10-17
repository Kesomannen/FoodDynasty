using System;
using Dynasty.Library.Extensions;
using UnityEngine;

namespace Dynasty.Core.Grid {

/// <summary>
/// Represents a 2D rotation in 90 degree steps.
/// </summary>
[Serializable]
public struct GridRotation {
    [Tooltip("The number of 90 degree steps on this rotation.")]
    [SerializeField] int _value;

    /// <summary>
    /// The number of 90 degree steps on this rotation.
    /// </summary>
    public int Steps => _value;
    
    /// <summary>
    /// The number of degrees on this rotation.
    /// </summary>
    public int Degrees => _value * 90;
    
    /// <summary>
    /// The number of radians on this rotation.
    /// </summary>
    public float Radians => _value * Mathf.PI / 2f;

    /// <summary>
    /// Constructs a new rotation with the given 90 degree steps applied.
    /// </summary>
    public GridRotation(int value) {
        _value = Clamp(value);
    }

    /// <summary>
    /// Constructs a new rotation.
    /// </summary>
    /// <param name="rotation">The rotation to convert.</param>
    /// <param name="axis">The axis to get rotation from.</param>
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

    /// <summary>
    /// Returns a copy with the given 90 degree steps added.
    /// </summary>
    public GridRotation Rotated(int steps) {
        return new GridRotation(_value + steps);
    }

    /// <summary>
    /// Constructs a quaternion from this rotation, rotating around the given axis.
    /// </summary>
    public Quaternion ToQuaternion(Vector3 axis) {
        return Quaternion.AngleAxis(Degrees, axis);
    }

    /// <summary>
    /// Rotates the given vector according to this rotation.
    /// </summary>
    public Vector2 Rotate(Vector2 vector) {
        for (var i = 0; i < _value; i++) {
            vector = vector.RotateCCW();
        }

        return vector;
    }

    /// <summary>
    /// Rotates the given vector according to this rotation.
    /// </summary>
    public Vector2Int Rotate(Vector2Int vector) {
        for (var i = 0; i < _value; i++) {
            vector = vector.RotateCCW();
        }

        return vector;
    }

    /// <summary>
    /// Adds the given 90 degree steps to this rotation.
    /// </summary>
    public static GridRotation operator +(GridRotation rotation, int value) {
        return rotation.Rotated(value);
    }

    public override string ToString() {
        return $"{Degrees}°";
    }
}

}