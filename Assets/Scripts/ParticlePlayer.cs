using Dynasty.Library;
using UnityEngine;

public class ParticlePlayer : MonoBehaviour {
    [SerializeField] ParticleSystem[] _particles;

    public void PlayParticles() {
        _particles.ForEach(particle => particle.Play());
    }
}