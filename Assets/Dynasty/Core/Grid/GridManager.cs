using System.Collections.Generic;
using System.Linq;
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

    Cell[,] _cells;

    void Awake() {
        _cells = new Cell[_gridSize.x, _gridSize.y];
        for (var x = 0; x < _gridSize.x; x++) {
            for (var y = 0; y < _gridSize.y; y++) {
                _cells[x, y] = new Cell(GridToWorld(x, y));
            }
        }
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

    void Add(GridObject gridObject, Vector2Int position, GridRotation rotation, IEnumerable<Cell> overlappingCells) {
        foreach (var cell in overlappingCells) {
            cell.Inhabitants.Add(gridObject);
        }
        
        gridObject.OnAdded(this, position, rotation);
    }

    void Remove(GridObject gridObject) {
        var cells = GetOverlapping(gridObject.GridPosition, gridObject.RotatedSize);
        foreach (var inhabitants in cells.Select(cell => cell.Inhabitants)) {
            inhabitants.Remove(gridObject);
        }
        
        gridObject.OnRemoved();
    }
    
    bool CheckOverlapping(Vector2Int position, GridSize size, out IEnumerable<Cell> overlappingCells) {
        if (!IsWithinGrid(position, size.Bounds)) {
            overlappingCells = null;
            return false;
        }

        overlappingCells = GetOverlapping(position, size);
        return overlappingCells.All(cell => cell.IsEmpty);
    }

    IEnumerable<Cell> GetOverlapping(Vector2Int position, GridSize size) {
        return size.GetBlockingPositions().Select(pos => _cells[position.x + pos.x, position.y + pos.y]);
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

    /// <summary>
    /// Retrieves all objects present in the grid.
    /// </summary>
    /// <remarks>Performance intensive; do not use in game loops.</remarks>
    public IEnumerable<GridObject> GetAllObjects() {
        var objects = new HashSet<GridObject>();

        foreach (var cell in _cells) {
            if (cell.IsEmpty) continue;
            foreach (var inhabitant in cell.Inhabitants.Where(obj => !objects.Contains(obj))) {
                objects.Add(inhabitant);
            }
        }
        
        return objects;
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

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        for (var x = 0; x < _gridSize.x; x++) {
            for (var y = 0; y < _gridSize.y; y++) {
                DrawCell(x, y);
            }
        }

        void DrawCell(int x, int y) {
            Vector3 worldPosition;

            if (Application.isPlaying) {
                if (!IsInGrid(new Vector2Int(x, y))) return;

                var cell = _cells[x, y];
                Gizmos.color = cell.IsEmpty ? Color.green : Color.red;
                worldPosition = cell.WorldPosition;
            } else {
                worldPosition = GridToWorld(x, y);
            }

            Gizmos.DrawWireCube(worldPosition, new Vector3(_cellSize.x, 0, _cellSize.y) * 0.95f);
        }
    }

    readonly struct Cell {
        public readonly Vector3 WorldPosition;
        public readonly List<GridObject> Inhabitants;
        public bool IsEmpty => Inhabitants.Count == 0;

        public Cell(Vector3 worldPosition) {
            WorldPosition = worldPosition;
            Inhabitants = new List<GridObject>();
        }
    }
}

}