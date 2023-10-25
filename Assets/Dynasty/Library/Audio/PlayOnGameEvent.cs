using Dynasty.Library;
using UnityEngine;

namespace Dynasty.Library {

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