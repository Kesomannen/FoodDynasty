using System.Collections;
using UnityEngine;

namespace Dynasty.Library.Audio {

public class IdleSound : MonoBehaviour {
    [SerializeField] SoundEffect _soundEffect;

    IEnumerator Start() {
        while (enabled) {
            yield return _soundEffect.PlayRoutine();
        }
    }
}

}