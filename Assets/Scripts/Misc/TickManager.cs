using System;
using UnityEngine;

public class TickManager : MonoBehaviour {
    int _tick;
    
    static TickManager _instance;

    static TickManager Instance {
        get {
            if (_instance != null) return _instance;
            
            _instance = FindObjectOfType<TickManager>();
            if (_instance != null) return _instance;
            
            _instance = new GameObject("TickManager").AddComponent<TickManager>();
            DontDestroyOnLoad(_instance);
            return _instance;
        }
    }
    
    public event Action OnTick;
    
    const float TickEvery = 1f;

    void FixedUpdate() {
        _tick++;
        if (_tick % TickEvery != 0) return;
        
        OnTick?.Invoke();
    }
    
    public static void AddListener(Action listener) {
        Instance.OnTick += listener;
    }
    
    public static void RemoveListener(Action listener) {
        Instance.OnTick -= listener;
    }
}