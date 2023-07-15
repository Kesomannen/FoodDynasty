using UnityEngine;

namespace Dynasty.Core.Inventory {

public interface IPrefabProvider<out T> where T : Component {
    T Prefab { get; }
}

}