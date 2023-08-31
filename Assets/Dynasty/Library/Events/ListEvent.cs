using System;
using System.Collections.Generic;
using GenericUnityObjects;

namespace Dynasty.Library.Events {

[CreateGenericAssetMenu(MenuName = "Event/List")]
public class ListEvent<T> : GenericGameEvent {
    readonly List<T> _list = new();
    
    event Action<T> OnAdded;
    event Action<T> OnRemoved;

    public void Clear() {
        while (_list.Count > 0) {
            Remove(_list[0]);
        }
    }

    public void AddListener(Action<T> onAdded, Action<T> onRemoved) {
        OnAdded += onAdded;
        OnRemoved += onRemoved;
        foreach (var val in _list) {
            onAdded(val);
        }
    }
    
    public void RemoveListener(Action<T> onAdded, Action<T> onRemoved) {
        OnAdded -= onAdded;
        OnRemoved -= onRemoved;
    }
    
    public void Add(T value) {
        _list.Add(value);
        OnAdded?.Invoke(value);
        Raise();
    }
    
    public void Remove(T value) {
        _list.Remove(value);
        OnRemoved?.Invoke(value);
    }
}

}

