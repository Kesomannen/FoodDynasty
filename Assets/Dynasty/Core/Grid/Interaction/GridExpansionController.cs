using System;
using System.Collections.Generic;
using System.Linq;
using Dynasty.Library.Extensions;
using UnityEngine;

namespace Dynasty.Core.Grid {

public class GridExpansionController : MonoBehaviour {
    [SerializeField] Vector2Int _expansionGridSize;
    [SerializeField] GridExpansion _expansionPrefab;
    [SerializeField] GridManager _grid;

    public bool[,] ExpansionGrid { get; private set; }
    
    readonly List<GridExpansion> _expansions = new();
    
    Vector2Int _expansionSize;
    Vector2Int _center;

    void Awake() {
        _expansionSize = _grid.GridSize;
        ExpansionGrid = new bool[_expansionGridSize.x, _expansionGridSize.y];
        _center = new Vector2Int(_expansionGridSize.x / 2, _expansionGridSize.y / 2);
        ExpansionGrid[_center.x, _center.y] = true;
    }

    void Start() {
        UpdatePreviews();
    }

    void Expand(Vector2Int position) {
        if (!IsInGrid(position)) {
            this.Error($"Position {position} is out of bounds.");
            return;
        }

        if (ExpansionGrid[position.x, position.y]) {
            this.Error($"Position {position} has already been expanded.");
            return;
        }
        
        var offset = (position - _center) * _expansionSize;
        _grid.Expand(_expansionSize, offset);
        ExpansionGrid[position.x, position.y] = true;
        
        _expansions.First(e => e.Position == position).Activate();
        UpdatePreviews();
    }

    void UpdatePreviews() {
        var dirs = new[] {
            (1, 0),
            (0, 1),
            (-1, 0),
            (0, -1)
        };
        
        for (var x = 0; x < _expansionGridSize.x; x++) {
            for (var y = 0; y < _expansionGridSize.y; y++) {
                if (!ExpansionGrid[x, y]) continue;

                foreach (var (xOffset, yOffset) in dirs) {
                    var pos = new Vector2Int(x + xOffset, y + yOffset);
                        
                    if (!IsInGrid(pos) || ExpansionGrid[pos.x, pos.y]) continue;
                    if (_expansions.Any(e => e.Position == pos)) continue;
                        
                    CreatePreview(pos);
                }
            }
        }
    }
    
    void CreatePreview(Vector2Int gridPos) {
        var centered = (gridPos - _center) * _expansionSize * _grid.CellSize;
        var pos = new Vector3(centered.x, 0, centered.y);
        
        var expansion = Instantiate(_expansionPrefab, transform);
        expansion.Initialize(gridPos);
        expansion.transform.localPosition = pos;
        expansion.OnClicked += OnExpansionClicked;
        
        _expansions.Add(expansion);
    }

    void OnExpansionClicked(GridExpansion expansion) {
        Expand(expansion.Position);
    }
    
    bool IsInGrid(Vector2Int position) {
        return position.x >= 0 && position.x < _expansionGridSize.x
            && position.y >= 0 && position.y < _expansionGridSize.y;
    }
}

}