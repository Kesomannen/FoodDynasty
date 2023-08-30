using System;
using System.Collections.Generic;
using System.Linq;
using Dynasty.Library.Helpers;
using UnityEngine;

namespace Dynasty.Core.Grid {

/// <summary>
/// Manages a grid of grid objects, and provides methods for adding and removing them.
/// </summary>
public class GridManager : MonoBehaviour {
    [Tooltip("World size of a single grid cell.")]
    [SerializeField] Vector2 _cellSize;
    
    [Tooltip("Size of the grid in cells.")]
    [SerializeField] Vector2Int _gridSize;

    readonly HashSet<GridObject> _gridObjects = new();
    int[,] _cells;

    public Vector2 CellSize => _cellSize;
    public Vector2Int GridSize => _gridSize;
    public IEnumerable<GridObject> GridObjects => _gridObjects;

    void Awake() {
        _cells = new int[_gridSize.x, _gridSize.y];
    }

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
        foreach (var vector2Int in overlapping) {
            ModifyCell(vector2Int, 1);
        }

        gridObject.OnAdded(this, position, rotation);
        _gridObjects.Add(gridObject);
    }

    void Remove(GridObject gridObject) {
        var cells = GetOverlapping(gridObject.GridPosition, gridObject.RotatedSize);
        foreach (var cell in cells) {
            ModifyCell(cell, -1);
        }
        
        gridObject.OnRemoved();
        _gridObjects.Remove(gridObject);
    }
    
    public void Expand(Vector2Int size, Vector2Int offset) {
        var newGridSize = _gridSize + size;
        var newCells = new int[newGridSize.x, newGridSize.y];
        
        for (var x = 0; x < _gridSize.x; x++) {
            for (var y = 0; y < _gridSize.y; y++) {
                newCells[x + offset.x, y + offset.y] = _cells[x, y];
            }
        }

        _cells = newCells;
        _gridSize = newGridSize;
    }
    
    bool CheckOverlapping(Vector2Int position, GridSize size, out IEnumerable<Vector2Int> overlapping) {
        if (!IsWithinGrid(position, size.Bounds)) {
            overlapping = null;
            return false;
        }

        overlapping = GetOverlapping(position, size);
        return overlapping.All(IsEmpty);
    }

    static IEnumerable<Vector2Int> GetOverlapping(Vector2Int position, GridSize size) {
        return size.GetBlockingPositions().Select(pos => pos + position);
    }
    
    bool IsWithinGrid(Vector2Int position, Vector2Int bounds) {
        return IsInGrid(position) && IsInGrid(position + bounds - Vector2Int.one);
    }
    
    bool IsInGrid(Vector2Int position) {
        return position.x >= 0 && position.x < _gridSize.x &&
               position.y >= 0 && position.y < _gridSize.y;
    }
    
    bool BelongsToGrid(GridObject gridObject) {
        return gridObject.IsPlaced && gridObject.GridManager is { } manager && manager == this;
    }
    
    int GetCell(Vector2Int position) {
        return _cells[position.x, position.y];
    }
    
    void ModifyCell(Vector2Int position, int value) {
        _cells[position.x, position.y] += value;
    }
    
    public bool IsEmpty(Vector2Int position) {
        return GetCell(position) == 0;
    }
    
    Vector3 GridToWorld(Vector2 position) {
        return transform.position + new Vector3(position.x * _cellSize.x, 0, position.y * _cellSize.y);
    }

    Vector3 GridToWorld(Vector2Int gridPosition, Vector2Int bounds) {
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
        return new Vector2Int(Mathf.RoundToInt(localPos.x / _cellSize.x), Mathf.RoundToInt(localPos.z / _cellSize.y));
    }
}

}