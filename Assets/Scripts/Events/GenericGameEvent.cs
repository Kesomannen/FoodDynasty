using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Event/Generic")]
public class GenericGameEvent : ScriptableObject {
    public event Action OnEventRaised;

    public void Raise() {
        OnEventRaised?.Invoke();
    }
    
    public void AddListener(Action listener) {
        OnEventRaised += listener;
    }
    
    public void RemoveListener(Action listener) {
        OnEventRaised -= listener;
    }
}