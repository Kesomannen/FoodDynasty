using System;
using UnityEngine;

public class GenericEvent : MonoBehaviour {
    public event Action OnRaisedGeneric;

    public void Raise() {
        OnRaisedGeneric?.Invoke();
    }
}