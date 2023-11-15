using System;
using System.Collections.Generic;
using Dynasty.Library;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;

namespace Dynasty.Grid {

/// <summary>
/// An object that lives on a grid.
/// </summary>
public class GridObject : MonoBehaviour, IInfoProvider {
    [Tooltip("Whether this object can be moved after being placed.")]
    [SerializeField] bool _canMove = true;
    
    [ShowIf("_canMove")] [AllowNesting]
    [Tooltip("Instantiated when showing a preview of this object.")]
    [SerializeField] GameObject _blueprintPrefab;
    
    [SerializeField] SoundEffect _placeSound;
    
    [Tooltip("Used when a grid rotation is applied to the object.")]
    [SerializeField] Vector3 _rotationAxis = Vector3.up;
    
    [Tooltip("The grid size of this object.")]
    [SerializeField] SerializedGridSize _size = new(4, 4);
    
    GridRotation _rotation;

    /// <summary>
    /// Sets the grid rotation of this object, and rotates the transform accordingly.
    /// </summary>
    public GridRotation Rotation {
        get => _rotation;
        private set {
            _rotation = value;
            transform.rotation = _rotation.ToQuaternion(_rotationAxis);
        }
    }
    
    /// <summary>
    /// The grid manager this object belongs to, or null if it is has not been placed yet.
    /// </summary>
    [CanBeNull]
    public GridManager GridManager { get; private set; }
    
    /// <summary>
    /// The grid position of this object.
    /// </summary>
    public Vector2Int GridPosition { get; set; }
    
    /// <summary>
    /// Whether this object is currently placed on a grid.
    /// </summary>
    public bool IsPlaced { get; private set; }

    /// <summary>
    /// This object's size, rotated according to its grid rotation.
    /// </summary>
    public GridSize RotatedSize => StaticSize.Rotated(Rotation.Steps);

    /// <summary>
    /// This object's static size, before any rotation is applied.
    /// </summary>
    public GridSize StaticSize {
        get => _size.Value;
        set => _size.Value = value;
    }
    
    /// <summary>
    /// The blueprint prefab used when showing a preview of this object.
    /// </summary>
    public GameObject BlueprintPrefab {
        get => _blueprintPrefab;
        set => _blueprintPrefab = value;
    }

    public SoundEffect PlaceSound {
        get => _placeSound;
        set => _placeSound = value;
    }
    
    /// <summary>
    /// Whether this object can be moved after being placed.
    /// </summary>
    public bool CanMove => _canMove;
    
    // Used when previewing the object's size in the editor.
    public static readonly Vector3 PreviewCellSize = new(0.25f, 0, 0.25f);
    
    public event Action OnAdded, OnRemoved; 
    
    /// <summary>
    /// Call to inform this object that it has been added to a grid.
    /// </summary>
    public void RegisterAdded(GridManager gridManager, Vector2Int gridPosition, GridRotation rotation) {
        GridManager = gridManager;
        GridPosition = gridPosition;
        Rotation = rotation;
        IsPlaced = true;
        
        OnAdded?.Invoke();
    }

    /// <summary>
    /// Call to inform this object that it has been removed from a grid.
    /// </summary>
    /// <remarks>Does not clear rotation, position or the reference to <see cref="GridManager"/>.</remarks>
    public void RegisterRemoved() {
        IsPlaced = false;
        
        OnRemoved?.Invoke();
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
            worldPos.x += gridPos.x * PreviewCellSize.x;
            worldPos.z += gridPos.y * PreviewCellSize.z;
            
            Gizmos.DrawWireCube(worldPos, PreviewCellSize);
        }
    }
}

}