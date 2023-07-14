using System;
using UnityEngine;
using UnityEngine.Events;

namespace Dynasty.Library.Events {

public class GenericEvent : MonoBehaviour {
    public event Action OnRaisedGeneric;

    public void Raise() {
        OnRaisedGeneric?.Invoke();
    }
}

}