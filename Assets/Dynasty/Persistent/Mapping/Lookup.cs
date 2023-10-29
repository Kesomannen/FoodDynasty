using System;
using System.Collections;
using System.Collections.Generic;
using GenericUnityObjects;
using UnityEngine;

namespace Dynasty.Persistent {

[CreateGenericAssetMenu(MenuName = "Saving/Lookup")]
public class Lookup<T> : ScriptableObject, IEnumerable<T> {
    [SerializeField] T[] _values;
    
    public T GetFromId(int id) => _values[id];
    
    public int GetId(T value) {
        var index = Array.IndexOf(_values, value);
        if (index != -1) return index;
        
        Debug.LogError($"Value {value} not found in lookup");
        return -1;
    }

    public IEnumerator<T> GetEnumerator() {
        return ((IEnumerable<T>) _values).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}

}