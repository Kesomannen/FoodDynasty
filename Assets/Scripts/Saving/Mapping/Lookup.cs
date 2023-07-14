using System;
using GenericUnityObjects;
using UnityEngine;

[CreateGenericAssetMenu(MenuName = "Saving/Lookup")]
public class Lookup<T> : ScriptableObject {
    [SerializeField] T[] _values;
    
    public T GetFromId(int id) => _values[id];
    public int GetId(T value) {
        var index = Array.IndexOf(_values, value);
        if (index != -1) return index;
        
        Debug.LogError($"Value {value} not found in lookup");
        return -1;
    }
}