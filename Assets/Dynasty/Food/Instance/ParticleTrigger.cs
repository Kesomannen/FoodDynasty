using Dynasty.Library.Classes;
using Dynasty.Library.Events;
using Dynasty.Library.Pooling;
using UnityEngine;

namespace Dynasty.Food.Instance {

public class ParticleTrigger : MonoBehaviour {
    [SerializeField] ParticleSpawner _spawner;
    [SerializeField] GenericEvent _trigger;
    [SerializeField] Optional<Transform> _spawnPoint;

    void OnEnable() {
        _trigger.OnRaisedGeneric += OnTriggered;
    }
    
    void OnDisable() {
        _trigger.OnRaisedGeneric -= OnTriggered;
    }
    
    void OnTriggered() {
        _spawner.SpawnAt(_spawnPoint.Enabled ? _spawnPoint.Value : transform);
    }
}

}