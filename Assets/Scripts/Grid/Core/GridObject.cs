using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour, IGridObject, IInfoProvider {
    [SerializeField] Vector3 _rotationAxis = Vector3.up;
    [SerializeField] GameObject _blueprintPrefab;
    [SerializeField] SerializedGridSize _size;
    
    GridRotation _rotation;

    public GridRotation Rotation {
        get => _rotation;
        private set {
            _rotation = value;
            transform.rotation = _rotation.ToQuaternion(_rotationAxis);
        }
    }

    public IGridManager GridManager { get; private set; }
    public Vector2Int GridPosition { get; private set; }
    public bool IsPlaced { get; private set; }

    public GridSize StaticSize => _size.Value ;
    public GameObject BlueprintPrefab => _blueprintPrefab;
    
    static readonly Vector3 _previewCellSize = new(0.25f, 0, 0.25f);
    
    public void OnAdded(IGridManager gridManager, Vector2Int gridPosition, GridRotation rotation) {
        GridManager = gridManager;
        GridPosition = gridPosition;
        Rotation = rotation;
        IsPlaced = true;
    }

    public void OnRemoved() {
        IsPlaced = false;
    }

    public IEnumerable<(string Name, string Value)> GetInfo() {
        yield return ("Size", $"{StaticSize.Bounds.x}x{StaticSize.Bounds.y}");
    }

    void OnDrawGizmos() {
        if (IsPlaced) return;
        
        Gizmos.color = Color.magenta;
        foreach (var cellPos in StaticSize.GetBlockingPositions()) {
            var gridPos = cellPos - (Vector2) (StaticSize.Bounds - Vector2Int.one) / 2f;
            
            var worldPos = transform.position;
            worldPos.x += gridPos.x * _previewCellSize.x;
            worldPos.z += gridPos.y * _previewCellSize.z;
            
            Gizmos.DrawWireCube(worldPos, _previewCellSize);
        }
    }
}