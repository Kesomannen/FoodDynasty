using System;
using UnityEngine;

public class GenericLocalEvent : MonoBehaviour {
    public event Action OnEventRaised;

    protected void Raise() {
        OnEventRaised?.Invoke();
    }
    
    public void AddListener(Action listener) {
        OnEventRaised += listener;
    }
    
    public void RemoveListener(Action listener) {
        OnEventRaised -= listener;
    }
}