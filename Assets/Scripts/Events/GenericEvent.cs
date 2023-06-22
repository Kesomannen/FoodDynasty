using System;
using UnityEngine;
using UnityEngine.Events;

public class GenericEvent : MonoBehaviour {
    public event Action OnRaisedGeneric;

    public void Raise() {
        OnRaisedGeneric?.Invoke();
    }
}