using UnityEngine;

public interface IGridObject {
    GridRotation Rotation { get; }
    Vector2Int GridPosition { get; }
    
    IGridManager GridManager { get; }
    GameObject BlueprintPrefab { get; }
    bool IsPlaced { get; }
    
    GridSize StaticSize { get; }
    GridSize RotatedSize => StaticSize.Rotated(Rotation.Steps);

    void OnAdded(IGridManager gridManager, Vector2Int gridPosition, GridRotation rotation);
    void OnRemoved();
}