using System;
using UnityEngine;

namespace Dynasty.Library {

public class PoolableComponent<T> : MonoBehaviour, IPoolable<PoolableComponent<T>> where T : Component {
    [SerializeField] T _component;
    
    public T Component => _component;

    public CustomObjectPool<PoolableComponent<T>> Pool { get; set; }
    
    public event Action<PoolableComponent<T>> OnDisposed;

    public void Dispose() {
        OnDisposed?.Invoke(this);
    }
}

}