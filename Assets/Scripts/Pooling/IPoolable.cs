using System;

public interface IPoolable<out T> : IDisposable {
    event Action<T> OnDisposed;
}