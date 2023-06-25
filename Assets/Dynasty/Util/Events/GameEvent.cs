using System;
using GenericUnityObjects;

[CreateGenericAssetMenu(MenuName = "Event/Game")]
public class GameEvent<T> : GenericGameEvent {
    public event Action<T> OnRaised; 

    public void Raise(T value) {
        OnRaised?.Invoke(value);
        Raise();
    }
}