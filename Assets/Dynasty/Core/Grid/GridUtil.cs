using UnityEngine;

namespace Dynasty.Core.Grid {

/// <summary>
/// Provides utility methods for grid objects.
/// </summary>
public static class GridUtil {
    /// <summary>
    /// Adds a grid object to the given grid manager and positions its transform.
    /// </summary>
    public static bool AddAndPosition(this GridObject gridObject, GridManager gridManager, Vector2Int position, GridRotation rotation) {
        if (!gridManager.TryAdd(gridObject, position, rotation)) return false;
        
        var transform = gridObject.transform;
        
        transform.position = gridManager.GridToWorld(position, gridObject.StaticSize, rotation);
        transform.SetParent(gridManager.transform, true);
        
        return true;
    }
}

}