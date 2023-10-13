using Dynasty.Library.Events;
using UnityEngine;

namespace Dynasty.Library.Audio {

public class PlayOnLocalEvent : MonoBehaviour {
    [SerializeField] SoundEffect _soundEffect;
    [SerializeField] GenericEvent _event;
    
    void OnEnable() {
        _event.OnRaisedGeneric += _soundEffect.Play;
    }
    
    void OnDisable() {
        _event.OnRaisedGeneric -= _soundEffect.Play;
    }
}

}