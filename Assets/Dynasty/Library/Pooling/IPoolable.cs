using System;
using UnityEngine;

namespace Dynasty.Library {

public interface IPoolable<T> : IDisposable where T : Component, IPoolable<T> {
    CustomObjectPool<T> Pool { get; set; }
    
    event Action<T> OnDisposed;
}

}