using System.Collections;
using UnityEngine;

namespace Dynasty.Library {

[CreateAssetMenu(menuName = "Pooling/Particle Spawner")]
public class ParticleSpawner : CustomObjectPool<PoolableComponent<ParticleSystem>> {
    public void Spawn(Vector3 position) {
        var particle = Get();
        
        particle.transform.position = position;
        particle.Component.Play();
        particle.StartCoroutine(DespawnAfterDuration());

        IEnumerator DespawnAfterDuration() {
            yield return CoroutineHelpers.Wait(particle.Component.main.duration);
            particle.Dispose();
        }
    }
    
    public void SpawnAt(Transform transform) {
        Spawn(transform.position);
    }
}

}