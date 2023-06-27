﻿using System;
using GenericUnityObjects;

[CreateGenericAssetMenu(MenuName = "Event/ValueChanged")]
public class ValueChangedEvent<T> : GenericGameEvent {
    public event Action<T, T> OnRaised;
    
    public void Raise(T previous, T current) {
        OnRaised?.Invoke(previous, current);
        Raise();
    }
}