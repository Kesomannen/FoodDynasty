using UnityEngine;

public interface IGridManager {
    bool TryAdd(IGridObject gridObject, Vector2Int position, GridRotation rotation);
    bool TryRemove(IGridObject gridObject);
    bool CanAdd(IGridObject gridObject, Vector2Int position, GridRotation rotation);

    Vector3 GridToWorld(Vector2Int gridPosition);
    Vector3 GridToWorld(Vector2Int gridPosition, Vector2Int size, GridRotation rotation);
    Vector3 GridToWorld(IGridObject gridObject);
    
    Vector2Int WorldToGrid(Vector3 worldPosition);
}