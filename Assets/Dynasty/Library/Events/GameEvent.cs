﻿using System;
using GenericUnityObjects;

namespace Dynasty.Library {

[CreateGenericAssetMenu(MenuName = "Event/Game")]
public class GameEvent<T> : GenericGameEvent {
    event Action<T> OnRaised;

    public void Raise(T value) {
        OnRaised?.Invoke(value);
        Raise();
    }
    
    public void AddListener(Action<T> listener) {
        OnRaised += listener;
    }
    
    public void RemoveListener(Action<T> listener) {
        OnRaised -= listener;
    }
#if UNITY_EDITOR
        //Domain Reload Compatible
        private void OnDisable()
        {
            OnRaised = null;
        }
#endif
    }
   
}