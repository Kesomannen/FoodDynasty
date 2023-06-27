using System;
using GenericUnityObjects;
using UnityEngine;

[CreateGenericAssetMenu(MenuName = "Saving/Lookup")]
public class Lookup<T> : ScriptableObject {
    [SerializeField] T[] _values;
    
    public int GetId(T value) => Array.IndexOf(_values, value);
    public T GetFromId(int id) => _values[id];
}