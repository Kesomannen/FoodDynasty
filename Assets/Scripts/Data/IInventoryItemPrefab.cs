using UnityEngine;

public interface IInventoryItemPrefab<out T> where T : Component {
    T Prefab { get; }
}