using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace Dynasty.Library {

public abstract class CustomObjectPool<T> : ScriptableObject, IDisposable where T : Component, IPoolable<T> {
    [Header("Object Pool")]
    [SerializeField] T _prefab;
    [SerializeField] bool _collectionCheck = true;
    [SerializeField] int _defaultCapacity = 10;
    [SerializeField] int _maxSize = 10000;
    [SerializeField] bool _clearOnSceneChange = true;

    readonly List<T> _all = new();
    ObjectPool<T> _pool;

    public T Prefab {
        get => _prefab;
        set => _prefab = value;
    }

    protected virtual ObjectPool<T> Pool => _pool ??= new ObjectPool<T>(
        Create, OnGet, OnRelease, Destroy, 
        _collectionCheck, 
        _defaultCapacity, 
        _maxSize
    );

    void OnEnable() {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }
    
    void OnDisable() {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
    
    void OnSceneUnloaded(Scene scene) {
        if (_clearOnSceneChange) {  
            Clear();
        }
    }

    protected virtual void OnGet(T obj) {
        obj.SetActive(true);
    }
    
    protected virtual void OnRelease(T obj) {
        obj.SetActive(false);
    }

    protected virtual T Create() {
        var obj = Instantiate(_prefab);
        DontDestroyOnLoad(obj);
        
        obj.Pool = this;
        obj.OnDisposed += Release;
        obj.SetActive(false);
        
        _all.Add(obj);
        return obj;
    }
    
    protected virtual void Destroy(T obj) {
        if (!_all.Remove(obj) || obj == null) return;
        
        obj.OnDisposed -= Release;
        Destroy(obj.gameObject);
    }

    public virtual void Dispose() {
        _pool?.Dispose();
    }
    
    void Clear() {
        while (_all.Count > 0) {
            Destroy(_all[^1]);
        }
        _all.Clear();
        _pool = null;
    }

    protected void Release(T obj) => Pool.Release(obj);
    public T Get() => Pool.Get();
}

}