using System;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public struct Optional<T> {
    [SerializeField] bool _enabled;
    [SerializeField] T _value;
    
    public Optional(T initialValue, bool enabled = true) {
        _enabled = enabled;
        _value = initialValue;
    }

    public bool Enabled => _enabled;
    public T Value => _value;
}