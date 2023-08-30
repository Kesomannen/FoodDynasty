using System;
using UnityEngine;

namespace Dynasty.Library.Classes {

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T> {
    static T _instance;
    static bool _isQuitting;
    
    public static T Instance {
        get {
            if (_isQuitting) return null;
            if (_instance != null) return _instance;
            
            _instance = FindObjectOfType<T>();
            if (_instance != null) return _instance;
            
            _instance = new GameObject(typeof(T).Name).AddComponent<T>();
            DontDestroyOnLoad(_instance);
            return _instance;
        }
    }
    
    public static bool Access(Action<T> action) { 
        if (_isQuitting) {
            return false;
        }
        
        action?.Invoke(Instance);
        return true;
    }

    void OnApplicationQuit() {
        _isQuitting = true;
    }
}

}