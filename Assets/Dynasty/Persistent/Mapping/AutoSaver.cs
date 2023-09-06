using System;
using System.Collections;
using Dynasty.Library.Helpers;
using Dynasty.Persistent.Core;
using UnityEngine;

# pragma warning disable 4014

namespace Dynasty.Persistent.Mapping {

public class AutoSaver : MonoBehaviour {
    [SerializeField] float _saveInterval = 60;
    [SerializeField] SaveManager _saveManager;

    async void Awake() {
        await _saveManager.LoadCurrent();
    }

    async void OnDestroy() {
        await _saveManager.SaveCurrent();
    }

    IEnumerator Start() {
        while (enabled) {
            yield return CoroutineHelpers.Wait(_saveInterval);
            _saveManager.SaveCurrent();   
        }
    }
}

}