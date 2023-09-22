using System;
using Dynasty.Library.Helpers;
using NaughtyAttributes;
using UnityEngine;

namespace Dynasty.Core.Grid {

[ExecuteInEditMode]
public class GridDebugger : MonoBehaviour {
    [SerializeField] GridManager _gridManager;
    [SerializeField] bool _drawGrid;
    [SerializeField] bool _selectCell;
    [ReadOnly] [AllowNesting]
    [SerializeField] Vector2Int _selectedCell;

    Plane _raycastPlane = new(Vector3.up, Vector3.zero);
    
    void Update() {
        if (!_selectCell || !_gridManager) return;
        var ray = MouseHelpers.ScreenRay;
        
        if (!_raycastPlane.Raycast(ray, out var distance)) return;
        
        var point = ray.GetPoint(distance);
        _selectedCell = _gridManager.WorldToGrid(point);
    }

    void OnDrawGizmos() {
        if (!_drawGrid || !_gridManager) return;
        var gridSize = _gridManager.GridSize;
        var cellSize = _gridManager.CellSize;
        
        for (var x = 0; x < gridSize.x; x++) {
            for (var y = 0; y < gridSize.y; y++) {
                DrawCell(new Vector2Int(x, y));
            }
        }

        return;

        void DrawCell(Vector2Int position) {
            if (_selectCell && position == _selectedCell) {
                Gizmos.color = Color.yellow;
            } else {
                Gizmos.color = _gridManager.GetState(position) switch {
                    GridManager.State.OutOfBounds => Color.red,
                    GridManager.State.Occupied => Color.yellow,
                    GridManager.State.Available => Color.green,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            Gizmos.DrawWireCube(_gridManager.GridToWorld(position), new Vector3(cellSize.x, 0, cellSize.y) * 0.95f);
        }
    }
}

}