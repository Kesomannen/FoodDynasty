using UnityEngine;

public interface IPrefabProvider<out T> where T : Component {
    T Prefab { get; }
}