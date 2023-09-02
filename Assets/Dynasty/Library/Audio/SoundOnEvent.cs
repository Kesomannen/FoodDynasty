using Dynasty.Library.Events;
using UnityEngine;

namespace Dynasty.Library.Audio {

public class SoundOnEvent : MonoBehaviour {
    [SerializeField] GenericEvent _event;
    [SerializeField] SoundEffect _soundEffect;

    void OnEnable() {
        _event.OnRaisedGeneric += OnRaised;
    }
    
    void OnDisable() {
        _event.OnRaisedGeneric -= OnRaised;
    }
    
    void OnRaised() {
        _soundEffect.Play();
    }
}

}