using Unity.Collections;
using UnityEngine;

public class GridObject : MonoBehaviour, IGridObject {
    [SerializeField] Vector2Int _size = new(1, 1);
    [SerializeField] Vector3 _rotationAxis = Vector3.up;
    [SerializeField] GameObject _blueprintPrefab;
    [SerializeField] [ReadOnly] GridRotation _rotation;

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

    public Vector2Int StaticSize => _size;
    public GameObject BlueprintPrefab => _blueprintPrefab;
    
    public void OnAdded(IGridManager gridManager, Vector2Int gridPosition, GridRotation rotation) {
        GridManager = gridManager;
        GridPosition = gridPosition;
        Rotation = rotation;
        IsPlaced = true;
    }

    public void OnRemoved() {
        IsPlaced = false;
    }
}