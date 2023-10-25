using System;
using UnityEngine;

namespace Dynasty.Library {

public class Poolable : MonoBehaviour, IPoolable<Poolable> {
    public event Action<Poolable> OnDisposed;
    
    public void Dispose() {
        transform.SetParent(null);
        OnDisposed?.Invoke(this);
    }
}

}