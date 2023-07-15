using System;
using Dynasty.Library.Classes;
using UnityEngine;

namespace Dynasty.Library {

public class TickManager : Singleton<TickManager> {
    public event Action OnTick;

    void FixedUpdate() {
        OnTick?.Invoke();
    }
    
    public static void AddListener(Action listener) {
        Access(tickManager => tickManager.OnTick += listener);
    }
    
    public static void RemoveListener(Action listener) {
        Access(tickManager => tickManager.OnTick -= listener);
    }
}

}