using System;
using UnityEngine;

public class Poolable : MonoBehaviour, IPoolable<Poolable> {
    public event Action<Poolable> OnDisposed;
    
    public void Dispose() {
        OnDisposed?.Invoke(this);
    }
}