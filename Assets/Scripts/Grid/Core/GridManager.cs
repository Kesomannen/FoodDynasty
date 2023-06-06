using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour, IGridManager {
    [SerializeField] Vector2 _cellSize;
    [SerializeField] Vector2Int _gridSize;

    Cell[,] _cells;

    void Awake() {
        _cells = new Cell[_gridSize.x, _gridSize.y];
        for (var x = 0; x < _gridSize.x; x++) {
            for (var y = 0; y < _gridSize.y; y++) {
                _cells[x, y] = new Cell(new Vector2Int(x, y), GridToWorld(x, y));
            }
        }
    }

    public bool TryAdd(IGridObject gridObject, Vector2Int position, GridRotation rotation) {
        if (gridObject.IsPlaced) return false;
        var rotatedSize = rotation.RotateSize(gridObject.StaticSize);
        if (!CheckOverlapping(position, rotatedSize, out var overlapping)) return false;
        
        Add(gridObject, position, rotation, overlapping);
        return true;
    }

    public bool TryRemove(IGridObject gridObject) {
        if (!BelongsToGrid(gridObject)) return false;
        
        Remove(gridObject);
        return true;
    }

    public bool CanAdd(IGridObject gridObject, Vector2Int position, GridRotation rotation) {
        var rotatedSize = rotation.RotateSize(gridObject.StaticSize);
        return CheckOverlapping(position, rotatedSize, out _);
    }

    void Add(IGridObject gridObject, Vector2Int position, GridRotation rotation, List<Cell> overlappingCells) {
        foreach (var cell in overlappingCells) {
            cell.Inhabitants.Add(gridObject);
        }
        
        gridObject.OnAdded(this, position, rotation);
    }

    void Remove(IGridObject gridObject) {
        var cells = GetOverlapping(gridObject.GridPosition, gridObject.RotatedSize);
        foreach (var inhabitants in cells.Select(cell => cell.Inhabitants)) {
            inhabitants.Remove(gridObject);
        }
        
        gridObject.OnRemoved();
    }
    
    bool CheckOverlapping(Vector2Int position, Vector2Int size, out List<Cell> overlappingCells) {
        overlappingCells = null;
        if (!IsWithinGrid(position, size)) {
            return false;
        }

        overlappingCells = GetOverlapping(position, size);
        return overlappingCells.All(cell => cell.IsEmpty);
    }

    List<Cell> GetOverlapping(Vector2Int position, Vector2Int size) {
        var cells = new List<Cell>();
        for (var x = position.x; x < position.x + size.x; x++) {
            for (var y = position.y; y < position.y + size.y; y++) {
                cells.Add(_cells[x, y]);
            }
        }

        return cells;
    }
    
    bool IsWithinGrid(Vector2Int position, Vector2Int size) {
        return IsInGrid(position) && IsInGrid(position + size - Vector2Int.one);
    }
    
    bool IsInGrid(Vector2Int position) {
        return position.x >= 0 && position.x < _gridSize.x &&
               position.y >= 0 && position.y < _gridSize.y;
    }
    
    bool BelongsToGrid(IGridObject gridObject) {
        return gridObject.IsPlaced && gridObject.GridManager is GridManager manager && manager == this;
    }

    public Vector3 GridToWorld(Vector2 position) {
        return transform.position + new Vector3(position.x * _cellSize.x, 0, position.y * _cellSize.y);
    }
    
    public Vector3 GridToWorld(Vector2Int gridPosition, Vector2Int rotatedSize) {
        return GridToWorld(gridPosition + (Vector2) (rotatedSize - Vector2Int.one) / 2f);
    }
    
    public Vector3 GridToWorld(Vector2Int gridPosition, Vector2Int size, GridRotation rotation) {
        return GridToWorld(gridPosition, rotation.RotateSize(size));
    }

    public Vector3 GridToWorld(IGridObject gridObject) {
        return GridToWorld(gridObject.GridPosition, gridObject.RotatedSize);
    }

    Vector3 GridToWorld(int x, int y) {
        return GridToWorld(new Vector2(x, y));
    }
    
    public Vector3 GridToWorld(Vector2Int gridPosition) {
        return GridToWorld((Vector2) gridPosition);
    }

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

            Gizmos.DrawWireCube(worldPosition, new Vector3(_cellSize.x, 0, _cellSize.y));
        }
    }

    readonly struct Cell {
        public readonly Vector2Int GridPosition;
        public readonly Vector3 WorldPosition;
        public readonly List<IGridObject> Inhabitants;
        public bool IsEmpty => Inhabitants.Count == 0;

        public Cell(Vector2Int gridPosition, Vector3 worldPosition) {
            GridPosition = gridPosition;
            WorldPosition = worldPosition;
            Inhabitants = new List<IGridObject>();
        }
    }
}