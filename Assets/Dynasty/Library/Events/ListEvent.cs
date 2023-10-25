using System;
using System.Collections.Generic;
using GenericUnityObjects;

namespace Dynasty.Library {

[CreateGenericAssetMenu(MenuName = "Event/List")]
public class ListEvent<T> : GenericGameEvent {
    readonly List<T> _items = new();
    
    public IReadOnlyList<T> Items => _items;
    
    event Action<T> OnAdded;
    event Action<T> OnRemoved;

    public void Clear() {
        while (_items.Count > 0) {
            Remove(_items[0]);
        }
    }

    public void AddListener(Action<T> onAdded, Action<T> onRemoved) {
        OnAdded += onAdded;
        OnRemoved += onRemoved;
        foreach (var val in _items) {
            onAdded(val);
        }
    }
    
    public void RemoveListener(Action<T> onAdded, Action<T> onRemoved) {
        OnAdded -= onAdded;
        OnRemoved -= onRemoved;
    }
    
    public void Add(T value) {
        _items.Add(value);
        OnAdded?.Invoke(value);
        Raise();
    }
    
    public void Remove(T value) {
        _items.Remove(value);
        OnRemoved?.Invoke(value);
    }
}

}

