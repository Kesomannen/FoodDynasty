﻿using System;
using Dynasty.Library;
using UnityEngine;

namespace Dynasty.Library {

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

}