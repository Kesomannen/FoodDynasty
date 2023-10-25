using System;
using System.Collections.Generic;
using Dynasty.Library;
using UnityEngine;

namespace Dynasty.Core.Grid {

/// <summary>
/// Represents a 2D size on a grid.
/// </summary>
[Serializable]
public struct GridSize {
    public GridSizeType Type;
    public Vector2Int Bounds;
    
    bool[,] _customMatrix;

    /// <summary>
    /// The custom matrix of blocking cells for this size.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the type of this size is not <see cref="GridSizeType.Custom"/>.</exception>
    public bool[,] CustomMatrix {
        get {
            if (Type != GridSizeType.Custom) {
                throw new InvalidOperationException("Cannot get custom matrix when type is not custom");
            }
            if (_customMatrix == null) {
                CreateMatrix(Bounds, true);
            }
            
            return _customMatrix;
        }
        set {
            if (Type != GridSizeType.Custom) return;
            _customMatrix = value;
        }
    }

    /// <summary>
    /// Constructs a new grid size.
    /// </summary>
    /// <param name="bounds">The bounds of the new size.</param>
    /// <param name="type">The type of the new size.</param>
    /// <param name="defaultValue">Will be the default value of the new custom matrix, if type is set to <see cref="GridSizeType.Custom"/>.</param>
    public GridSize(Vector2Int bounds, GridSizeType type = GridSizeType.Filled, bool defaultValue = true) {
        Type = type;
        Bounds = bounds;
        _customMatrix = null;

        if (Type == GridSizeType.Custom) 
            CreateMatrix(bounds, defaultValue);
    }

    void CreateMatrix(Vector2Int bounds, bool defaultValue) {
        _customMatrix = new bool[bounds.x, bounds.y];
        for (var x = 0; x < bounds.x; x++) {
            for (var y = 0; y < bounds.y; y++) {
                _customMatrix[x, y] = defaultValue;
            }
        }
    }

    /// <summary>
    /// Retrieves a collection of all relative grid positions that are blocked by this size.
    /// </summary>
    public IEnumerable<Vector2Int> GetBlockingPositions() {
        for (var x = 0; x < Bounds.x; x++) {
            for (var y = 0; y < Bounds.y; y++) {
                if (Type == GridSizeType.Custom && !_customMatrix[x, y]) continue;
                yield return new Vector2Int(x, y);
            }
        }
    }
    
    /// <summary>
    /// Returns a copy with the given 90 degree steps added.
    /// </summary>
    public GridSize Rotated(int steps) {
        var bounds = Bounds.RotateAbs(steps);
        var rotatedSize = new GridSize(bounds, Type);

        if (Type == GridSizeType.Custom) {
            rotatedSize._customMatrix = _customMatrix.RotateCW(steps);
            return rotatedSize;
        }

        return rotatedSize;
    }

    /// <summary>
    /// Sets the value of the custom matrix at the given relative position.
    /// </summary>
    public void Set(Vector2Int pos, bool value) {
        if (Type != GridSizeType.Custom) return;
        _customMatrix.SetAt(pos, value);
    }
    
    /// <summary>
    /// Returns true if this size blocks the given relative grid position.
    /// </summary>
    public bool Get(Vector2Int pos) {
        return Type != GridSizeType.Custom
               || _customMatrix.GetAt(pos);
    }

    public override string ToString() {
        return $"GridSize: {Type} {Bounds}";
    }
    
    public bool Equals(GridSize other) {
        return Type == other.Type &&
               Bounds == other.Bounds &&
               (Type != GridSizeType.Custom || _customMatrix.Equals(other._customMatrix));
    }

    public override int GetHashCode() {
        return Type == GridSizeType.Custom
            ? HashCode.Combine(Type, Bounds, _customMatrix)
            : HashCode.Combine(Type, Bounds);
    }
}

/// <summary>
/// The type of a <see cref="GridSize"/>.
/// </summary>
public enum GridSizeType {
    /// <summary>
    /// The size will block all grid positions within its bounds.
    /// </summary>
    Filled,
    
    /// <summary>
    /// The size will block grid positions according to a matrix of booleans.
    /// </summary>
    Custom
}

}