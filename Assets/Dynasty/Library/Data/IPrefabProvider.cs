using UnityEngine;

namespace Dynasty.Library {

/// <summary>
/// Provides a prefab, see <see cref="IDataProvider{T}"/>.
/// </summary>
public interface IPrefabProvider<out T> : IDataProvider<T> where T : Component { 
    T Prefab { get; }
    T IDataProvider<T>.Data => Prefab;
}

}