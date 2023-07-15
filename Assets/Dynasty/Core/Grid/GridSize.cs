using System;
using System.Collections.Generic;
using Dynasty.Library.Extensions;
using UnityEngine;

namespace Dynasty.Core.Grid {

[Serializable]
public struct GridSize {
    bool[,] _customMatrix;
    public GridSizeType Type;
    public Vector2Int Bounds;

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

    public GridSize(Vector2Int bounds, GridSizeType type = GridSizeType.Filled, bool defaultValue = true) {
        Type = type;
        Bounds = bounds;
        _customMatrix = null;

        if (Type != GridSizeType.Custom) return;
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

    public IEnumerable<Vector2Int> GetBlockingPositions() {
        for (var x = 0; x < Bounds.x; x++) {
            for (var y = 0; y < Bounds.y; y++) {
                if (Type == GridSizeType.Custom && !_customMatrix[x, y]) continue;
                yield return new Vector2Int(x, y);
            }
        }
    }
    
    public GridSize Rotated(int steps) {
        var bounds = Bounds.RotateAbs(steps);
        var rotatedSize = new GridSize(bounds, Type);

        if (Type != GridSizeType.Custom) return rotatedSize;
        rotatedSize._customMatrix = _customMatrix.RotateCW(steps);
        return rotatedSize;
    }

    public void Set(Vector2Int pos, bool value) {
        if (Type != GridSizeType.Custom) return;
        _customMatrix.SetAt(pos, value);
    }
    
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

}

public enum GridSizeType {
    Filled,
    Custom
}