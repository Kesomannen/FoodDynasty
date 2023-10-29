using System;
using UnityEngine;

namespace Dynasty.Grid {

/// <summary>
/// Serializable wrapper for <see cref="GridSize"/>.
/// </summary>
[Serializable]
public class SerializedGridSize : ISerializationCallbackReceiver {
    [SerializeField] Row[] _customMatrix;
    [SerializeField] GridSize _gridSize;

    /// <summary>
    /// Constructs a new wrapper for a filled grid size with the given bounds.
    /// </summary>
    public SerializedGridSize(int x, int y) {
        _gridSize = new GridSize(new Vector2Int(x, y));
    }
    
    /// <summary>
    /// Constructs a new wrapper for the given grid size.
    /// </summary>
    public SerializedGridSize(GridSize gridSize) {
        _gridSize = gridSize;
    }
    
    public GridSize Value {
        get => _gridSize;
        set => _gridSize = value;
    }

    public void OnBeforeSerialize() {
        if (_gridSize.Type != GridSizeType.Custom || _gridSize.CustomMatrix.Length == 0) return;
        
        _customMatrix = new Row[_gridSize.Bounds.x];
        for (var x = 0; x < _gridSize.Bounds.x; x++) {
            _customMatrix[x] = new Row { Values = new bool[_gridSize.Bounds.y] };
            for (var y = 0; y < _gridSize.Bounds.y; y++) {
                _customMatrix[x].Values[y] = _gridSize.CustomMatrix[x, y];
            }
        }
    }

    public void OnAfterDeserialize() { 
        if (_gridSize.Type != GridSizeType.Custom || _customMatrix.Length == 0) return;
        
        var matrix = new bool[_customMatrix.Length, _customMatrix[0].Values.Length];
        for (var x = 0; x < _customMatrix.Length; x++) {
            for (var y = 0; y < _customMatrix[x].Values.Length; y++) {
                matrix[x, y] = _customMatrix[x].Values[y];
            }
        } 
        _gridSize.CustomMatrix = matrix;
    }
    
    [Serializable]
    struct Row {
        public bool[] Values;
    }
}

}