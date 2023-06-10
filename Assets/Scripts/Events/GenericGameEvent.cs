using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Event/Generic")]
public class GenericGameEvent : ScriptableObject {
    public event Action OnRaisedGeneric;

    public void Raise() {
        OnRaisedGeneric?.Invoke();
    }
}