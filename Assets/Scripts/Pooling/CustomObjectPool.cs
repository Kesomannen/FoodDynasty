using GenericUnityObjects;
using UnityEngine;
using UnityEngine.Pool;

public abstract class CustomObjectPool<T> : ScriptableObject where T : Component, IPoolable<T> {
    [Header("Object Pool")]
    [SerializeField] T _prefab;
    [SerializeField] bool _collectionCheck = true;
    [SerializeField] int _defaultCapacity = 10;
    [SerializeField] int _maxSize = 10000;
    
    ObjectPool<T> _pool;

    protected virtual ObjectPool<T> Pool => _pool ??= new ObjectPool<T>(
        Create, OnGet, OnRelease, Destroy, 
        _collectionCheck, 
        _defaultCapacity, 
        _maxSize
    );
    
    protected virtual void OnGet(T obj) {
        obj.gameObject.SetActive(true);
    }
    
    protected virtual void OnRelease(T obj) {
        obj.gameObject.SetActive(false);
    }

    protected virtual T Create() {
        var obj = Instantiate(_prefab);
        DontDestroyOnLoad(obj.gameObject);
        obj.OnDisposed += Release;

        obj.gameObject.SetActive(false);
        return obj;
    }
    
    protected virtual void Destroy(T obj) {
        obj.OnDisposed -= Release;
        Destroy(obj.gameObject);
    }
    
    protected void Release(T obj) => Pool.Release(obj);
    public T Get() => Pool.Get();
}