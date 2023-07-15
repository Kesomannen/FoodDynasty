using System;

namespace Dynasty.Library.Pooling {

public interface IPoolable<out T> : IDisposable {
    event Action<T> OnDisposed;
}

}