using System;
using Dynasty.Library;
using UnityEngine;

namespace Dynasty.Library {

public class TickManager : Singleton<TickManager> {
    public event Action<float> OnTick;
    public event Action<float> OnSparseTick;

    const int SparseTickRate = 15;
    
    int _tickCount;

    void FixedUpdate() {
        _tickCount++;
        OnTick?.Invoke(Time.fixedDeltaTime);
        
        if (_tickCount % SparseTickRate == 0) {
            OnSparseTick?.Invoke(Time.fixedDeltaTime * SparseTickRate);
        }
    }
    
    public static void AddListener(Action<float> listener, bool sparse) {
        if (sparse) AddSparseListener(listener);
        else AddListener(listener);
    }
    
    public static void RemoveListener(Action<float> listener, bool sparse) {
        if (sparse) RemoveSparseListener(listener);
        else RemoveListener(listener);
    }

    public static void AddListener(Action<float> listener) {
        Access(tickManager => tickManager.OnTick += listener);
    }
    
    public static void RemoveListener(Action<float> listener) {
        Access(tickManager => tickManager.OnTick -= listener);
    }
    
    public static void AddSparseListener(Action<float> listener) {
        Access(tickManager => tickManager.OnSparseTick += listener);
    }
    
    public static void RemoveSparseListener(Action<float> listener) {
        Access(tickManager => tickManager.OnSparseTick -= listener);
    }
}

}