using System;
using System.Collections;
using Dynasty.Library.Helpers;
using Dynasty.Persistent.Core;
using UnityEngine;

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
        yield return CoroutineHelpers.Wait(_saveInterval);
        _saveManager.SaveCurrent();
    }
}

}