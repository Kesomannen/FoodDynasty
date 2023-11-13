using System;
using UnityEngine;

namespace Dynasty.Library {

public class PhysicsEvents : MonoBehaviour {
    public event Action<Collider> OnTriggerEnterEvent, OnTriggerExitEvent;
    
    void OnTriggerEnter(Collider other) {
        OnTriggerEnterEvent?.Invoke(other);
    }
    
    void OnTriggerExit(Collider other) {
        OnTriggerExitEvent?.Invoke(other);
    }
}

}