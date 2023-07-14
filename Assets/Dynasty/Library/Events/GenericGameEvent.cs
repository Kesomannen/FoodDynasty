using System;
using UnityEngine;

namespace Dynasty.Library.Events {

[CreateAssetMenu(menuName = "Event/Generic")]
public class GenericGameEvent : ScriptableObject {
    event Action OnRaised;

    public void Raise() {
        OnRaised?.Invoke();
    }
    
    public void AddListener(Action listener) {
        OnRaised += listener;
    }
    
    public void RemoveListener(Action listener) {
        OnRaised -= listener;
    }
}

}