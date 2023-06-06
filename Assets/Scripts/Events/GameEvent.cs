using System;
using GenericUnityObjects;

[CreateGenericAssetMenu]
public class GameEvent<T> : GenericGameEvent {
    public new event Action<T> OnEventRaised; 

    public void Raise(T value) {
        OnEventRaised?.Invoke(value);
        Raise();
    }
    
    public void AddListener(Action<T> listener) {
        OnEventRaised += listener;
    }
    
    public void RemoveListener(Action<T> listener) {
        OnEventRaised -= listener;
    }
}