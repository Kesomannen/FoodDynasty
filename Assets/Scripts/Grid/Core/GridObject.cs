using System.Collections.Generic;
using Dynasty.Library.Classes;
using NaughtyAttributes;
using UnityEngine;

public class GridObject : MonoBehaviour, IInfoProvider {
    [SerializeField] bool _canMove = true;
    [ShowIf("_canMove")] [AllowNesting]
    [SerializeField] GameObject _blueprintPrefab;
    [SerializeField] Vector3 _rotationAxis = Vector3.up;
    [SerializeField] SerializedGridSize _size;
    
    GridRotation _rotation;

    public GridRotation Rotation {
        get => _rotation;
        private set {
            _rotation = value;
            transform.rotation = _rotation.ToQuaternion(_rotationAxis);
        }
    }
    
    public GridManager GridManager { get; private set; }
    public Vector2Int GridPosition { get; private set; }
    public bool IsPlaced { get; private set; }

    public GridSize StaticSize => _size.Value;
    public GridSize RotatedSize => StaticSize.Rotated(Rotation.Steps);
    
    public GameObject BlueprintPrefab {
        get => _blueprintPrefab;
        set => _blueprintPrefab = value;
    }

    public Vector3 RotationAxis => _rotationAxis;
    public bool CanMove => _canMove;
    
    static readonly Vector3 _previewCellSize = new(0.25f, 0, 0.25f);
    
    public void OnAdded(GridManager gridManager, Vector2Int gridPosition, GridRotation rotation) {
        GridManager = gridManager;
        GridPosition = gridPosition;
        Rotation = rotation;
        IsPlaced = true;
    }

    public void OnRemoved() {
        IsPlaced = false;
    }

    public IEnumerable<EntityInfo> GetInfo() {
        yield return new EntityInfo("Size", $"{StaticSize.Bounds.x}x{StaticSize.Bounds.y}");
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