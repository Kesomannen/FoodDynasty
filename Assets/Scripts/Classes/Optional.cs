using System;
using NaughtyAttributes;
using UnityEngine;

public interface IOptional<out T> {
    bool Enabled { get; }
    T Value { get; }
}

[Serializable]
public struct Optional<T> : IOptional<T> {
    [SerializeField] bool _enabled;
    [SerializeField] T _value;
    
    public Optional(T initialValue, bool enabled = true) {
        _enabled = enabled;
        _value = initialValue;
    }

    public bool Enabled => _enabled;
    public T Value => _value;
}