using System;
using System.Collections;
using UnityEngine;

namespace Dynasty.Library.Audio {

public class IdleSound : MonoBehaviour {
    [SerializeField] SoundEffect _soundEffect;

    Action _cancel;
    
    void OnEnable() {
        StartCoroutine(Routine());
        return;

        IEnumerator Routine() {
            while (enabled) {
                yield return _soundEffect.Play(out _cancel);
            }
        }
    }

    void OnDisable() {
        _cancel?.Invoke();
    }
}

}