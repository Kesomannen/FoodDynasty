using System;

namespace Dynasty.Library {

public interface IPoolable<out T> : IDisposable {
    event Action<T> OnDisposed;
}

}