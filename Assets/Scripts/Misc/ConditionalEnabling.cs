using System.Collections;
using Dynasty.Library.Events;
using Dynasty.Library.Helpers;
using UnityEngine;

public class ConditionalEnabling : MonoBehaviour {
    [SerializeField] float _updateEvery = 1f;
    [SerializeField] CheckEvent<bool> _conditional;
    [SerializeField] Behaviour[] _behaviours;
    [SerializeField] ParticleSystem[] _particleSystems;

    void OnEnable() {
        StartCoroutine(UpdateLoop());
    }

    IEnumerator UpdateLoop() {
        while (enabled) {
            UpdateState();
            yield return CoroutineHelpers.Wait(_updateEvery);
        }
    }

    void UpdateState() {
        var active = _conditional.Check();
        
        foreach (var behaviour in _behaviours) {
            behaviour.enabled = active;
        }

        foreach (var particle in _particleSystems) {
            if (active) particle.Play();
            else particle.Stop();
        }
    }
}