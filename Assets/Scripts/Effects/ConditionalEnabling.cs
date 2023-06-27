﻿using System;
using System.Collections;
using UnityEngine;

public class ConditionalEnabling : MonoBehaviour {
    [SerializeField] float _updateEvery = 1f;
    [SerializeField] Condition _conditional;
    [SerializeField] Behaviour[] _behaviours;
    [SerializeField] ParticleSystem[] _particleSystems;

    void OnEnable() {
        StartCoroutine(UpdateLoop());
    }

    IEnumerator UpdateLoop() {
        while (enabled) {
            Update();
            yield return CoroutineHelpers.Wait(_updateEvery);
        }
    }

    void Update() {
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