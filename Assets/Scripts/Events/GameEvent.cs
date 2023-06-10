using System;
using GenericUnityObjects;

[CreateGenericAssetMenu]
public class GameEvent<T> : GenericGameEvent {
    public event Action<T> OnRaised; 

    public void Raise(T value) {
        OnRaised?.Invoke(value);
        Raise();
    }
}