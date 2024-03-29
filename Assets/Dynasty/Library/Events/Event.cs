﻿using System;
using UnityEngine;

namespace Dynasty.Library {

public class Event<T> : GenericEvent {
    public event Action<T> OnRaised;

    public void Raise(T value) {
        OnRaised?.Invoke(value);
        Raise();
    }
}

}