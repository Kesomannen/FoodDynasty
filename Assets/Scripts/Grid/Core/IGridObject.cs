using UnityEngine;

public interface IGridObject {
    GridRotation Rotation { get; }
    Vector2Int GridPosition { get; }
    
    IGridManager GridManager { get; }
    GameObject BlueprintPrefab { get; }
    bool IsPlaced { get; }
    
    Vector2Int StaticSize { get; }
    Vector2Int RotatedSize => Rotation.RotateSize(StaticSize);

    void OnAdded(IGridManager gridManager, Vector2Int gridPosition, GridRotation rotation);
    void OnRemoved();
}