using System;
using System.Collections.Generic;
using System.Linq;
using Dynasty.Library;
using UnityEngine;

namespace Dynasty.Grid {

/// <summary>
/// Manages a grid of grid objects, and provides methods for adding and removing them.
/// </summary>
public class GridManager : MonoBehaviour {
    [Tooltip("World size of a single grid cell.")]
    [SerializeField] Vector2 _cellSize;
    
    [Tooltip("Size of the grid in cells.")]
    [SerializeField] Vector2Int _gridSize;

    readonly HashSet<GridObject> _gridObjects = new();
    
    int?[,] _cells;

    public int?[,] Cells {
        get {
            if (_cells != null) return _cells;
            return _cells = CreateGrid();
        }
        private set => _cells = value;
    }

    public Vector2 CellSize => _cellSize;
    public Vector2Int GridSize => _gridSize;
    public IEnumerable<GridObject> GridObjects => _gridObjects;
    
    public event Action<GridObject, Vector2Int, GridRotation> OnObjectAdded;
    public event Action<GridObject> OnObjectRemoved;

    /// <summary>
    /// Attempts to add a grid object to the grid.
    /// </summary>
    /// <param name="gridObject">The grid object.</param>
    /// <param name="position">Grid position of the object.</param>
    /// <param name="rotation">Grid rotation of the object.</param>
    /// <returns>True if the placement was unobstructed.</returns>
    /// <remarks>Does not move object transform. See <see cref="GridUtil.AddAndPosition"/></remarks>
    public bool TryAdd(GridObject gridObject, Vector2Int position, GridRotation rotation) {
        if (gridObject.IsPlaced) return false;
        
        var rotatedSize = gridObject.StaticSize.Rotated(rotation.Steps);
        if (!CheckOverlapping(position, rotatedSize, out var overlapping)) return false;
        
        Add(gridObject, position, rotation, overlapping);
        return true;
    }

    /// <summary>
    /// Attempts to remove a grid object from the grid.
    /// </summary>
    /// <returns>True if the object was successfully removed.</returns>
    /// <remarks>Does not destroy or modify object's transform.</remarks>
    public bool TryRemove(GridObject gridObject) {
        if (!BelongsToGrid(gridObject)) return false;
        
        Remove(gridObject);
        return true;
    }

    /// <summary>
    /// Determines whether a grid object can be added to the grid at the given position and rotation.
    /// </summary>
    public bool CanAdd(GridObject gridObject, Vector2Int position, GridRotation rotation) {
        var rotatedSize = gridObject.StaticSize.Rotated(rotation.Steps);
        return CheckOverlapping(position, rotatedSize, out _);
    }

    void Add(GridObject gridObject, Vector2Int position, GridRotation rotation, IEnumerable<Vector2Int> overlapping) {
        foreach (var cell in overlapping) {
            AddOccupants(cell, 1);
        }

        gridObject.RegisterAdded(this, position, rotation);
        _gridObjects.Add(gridObject);
        
        OnObjectAdded?.Invoke(gridObject, position, rotation);
    }

    void Remove(GridObject gridObject) {
        var cells = GetOverlapping(gridObject.GridPosition, gridObject.RotatedSize);
        foreach (var cell in cells) {
            AddOccupants(cell, -1);
        }
        
        gridObject.RegisterRemoved();
        _gridObjects.Remove(gridObject);
        
        OnObjectRemoved?.Invoke(gridObject);
    }
    
    public void SetSize(Vector2Int newSize) {
        if (newSize.x <= _gridSize.x || newSize.y <= _gridSize.y) {
            Debug.LogError("Cannot shrink grid.");
            return;
        }
        
        var oldSize = _gridSize;
        _gridSize = newSize;
        var offset = (oldSize - _gridSize) / 2;

        var newCells = CreateGrid();
        for (var x = 0; x < oldSize.x; x++) {
            for (var y = 0; y < oldSize.y; y++) {
                var cell = Cells[x, y];
                if (cell is null or 0) continue; 
                newCells[x - offset.x, y - offset.y] = cell;
            }
        }
        
        foreach (var gridObject in GridObjects) {
            gridObject.GridPosition -= offset;
        }
        
        Cells = newCells;
    }
    
    public bool CheckOverlapping(Vector2Int position, GridSize size, out IEnumerable<Vector2Int> overlapping) {
        if (!IsWithinGrid(position, size.Bounds)) {
            overlapping = null;
            return false;
        }

        overlapping = GetOverlapping(position, size);
        return overlapping.All(IsAvailable);
    }
    
    int?[,] CreateGrid() {
        var cells = new int?[_gridSize.x, _gridSize.y];

        for (var x = 0; x < _gridSize.x; x++) {
            for (var y = 0; y < _gridSize.y; y++) {
                cells[x, y] = 0;
            }
        }
        
        return cells;
    }

    static IEnumerable<Vector2Int> GetOverlapping(Vector2Int position, GridSize size) {
        return size.GetBlockingPositions().Select(pos => pos + position);
    }
    
    bool IsWithinGrid(Vector2Int position, Vector2Int bounds) {
        return IsInGrid(position) && IsInGrid(position + bounds - Vector2Int.one);
    }
    
    bool IsInGrid(Vector2Int position) {
        return position.x >= 0 && position.x < _gridSize.x && position.y >= 0 && position.y < _gridSize.y;
    }
    
    bool BelongsToGrid(GridObject gridObject) {
        return gridObject.IsPlaced && gridObject.GridManager == this;
    }
    
    int? GetCell(Vector2Int position) {
        return Cells[position.x, position.y];
    }
    
    bool IsAvailable(Vector2Int position) {
        return GetCell(position) == 0;
    }
    
    void AddOccupants(Vector2Int position, int value) {
        Cells[position.x, position.y] += value;
    }

    public CellState GetState(Vector2Int position) {
        if (!IsInGrid(position)) return CellState.OutOfBounds;
        return IsAvailable(position) ? CellState.Available : CellState.Occupied;
    }
    
    Vector3 GridToWorld(Vector2 position) {
        position -= _gridSize / 2;
        return transform.position + new Vector3(position.x * _cellSize.x, 0, position.y * _cellSize.y);
    }

    public Vector3 GridToWorld(Vector2Int gridPosition, Vector2Int bounds) {
        return GridToWorld(gridPosition + (Vector2) (bounds - Vector2Int.one) / 2f);
    }
    
    /// <summary>
    /// Converts a grid position along with a size and rotation to the corresponding world position.
    /// </summary>
    public Vector3 GridToWorld(Vector2Int gridPosition, GridSize size, GridRotation rotation) {
        return GridToWorld(gridPosition, size.Rotated(rotation.Steps).Bounds);
    }

    /// <summary>
    /// Converts a grid object's position, rotation and size to the corresponding world position.
    /// </summary>
    public Vector3 GridToWorld(GridObject gridObject) {
        return GridToWorld(gridObject.GridPosition, gridObject.RotatedSize.Bounds);
    }

    Vector3 GridToWorld(int x, int y) {
        return GridToWorld(new Vector2(x, y));
    }
    
    /// <summary>
    /// Converts a grid position to the corresponding world position, ignoring rotation and size.
    /// </summary>
    public Vector3 GridToWorld(Vector2Int gridPosition) {
        return GridToWorld((Vector2) gridPosition);
    }

    /// <summary>
    /// Converts a world position to the corresponding grid position.
    /// </summary>
    public Vector2Int WorldToGrid(Vector3 worldPosition) {
        var localPos = worldPosition - transform.position;
        var gridPos = new Vector2Int(Mathf.RoundToInt(localPos.x / _cellSize.x), Mathf.RoundToInt(localPos.z / _cellSize.y));
        gridPos += _gridSize / 2;
        return gridPos;
    }

    public enum CellState {
        OutOfBounds,
        Occupied,
        Available
    }

    void OnValidate() {
        if (_cells != null) {
            var oldSize = new Vector2Int(_cells.GetLength(0), _cells.GetLength(1));
            if (oldSize == _gridSize) return;
        }
        _cells = CreateGrid();
    }
}

}