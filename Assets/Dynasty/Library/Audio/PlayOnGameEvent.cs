using Dynasty.Library.Events;
using UnityEngine;

namespace Dynasty.Library.Audio {

public class PlayOnGameEvent : MonoBehaviour {
    [SerializeField] SoundEffect _soundEffect;
    [SerializeField] GenericGameEvent _gameEvent;
    
    void OnEnable() {
        _gameEvent.AddListener(_soundEffect.Play);
    }
    
    void OnDisable() {
        _gameEvent.RemoveListener(_soundEffect.Play);
    }
}

}