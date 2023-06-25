using GenericUnityObjects;
using UnityEngine;

[CreateGenericAssetMenu(MenuName = "Pooling/UI")]
public class UIObjectPool<T> : CustomObjectPool<T> where T : Component, IPoolable<T> {
    Transform _storageParent;

    Transform StorageParent {
        get {
            if (_storageParent != null) return _storageParent;
            
            var storage = new GameObject($"{typeof(T).Name} Storage");
            DontDestroyOnLoad(storage);
            _storageParent = storage.transform;
            
            return _storageParent;
        }
    }
    
    public T Get(Transform parent) {
        var obj = Get();
        obj.transform.SetParent(parent);
        return obj;
    }

    protected override void OnGet(T obj) { }

    protected override void OnRelease(T obj) {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(StorageParent);
    }

    protected override T Create() {
        var obj = base.Create();
        obj.transform.SetParent(StorageParent);
        return obj;
    }
}