using UnityEngine;

namespace Dynasty.Library {

[CreateAssetMenu(menuName = "Pooling/Particle Spawner")]
public class ParticleSpawner : ScriptableObject {
    [SerializeField] ParticleSystem _prefab;

    ParticleSystem _particle;

    public void Spawn(Vector3 position) {
        if (_particle == null) {
            _particle = Instantiate(_prefab);
        }

        _particle.transform.position = position;
        _particle.Play();
    }
    
    public void SpawnAt(Transform transform) {
        Spawn(transform.position);
    }
}

}