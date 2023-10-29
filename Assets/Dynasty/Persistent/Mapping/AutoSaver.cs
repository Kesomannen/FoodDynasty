using System;
using System.Collections;
using Dynasty.Library;
using Dynasty.Persistent;
using UnityEngine;

# pragma warning disable 4014

namespace Dynasty.Persistent {

public class AutoSaver : MonoBehaviour {
    [SerializeField] float _saveInterval = 60;
    [SerializeField] SaveManager _saveManager;

    void Awake() {
        _saveManager.LoadCurrent();
    }

    void OnDestroy() {
        _saveManager.SaveCurrent();
    }

    IEnumerator Start() {
        while (enabled) {
            yield return CoroutineHelpers.Wait(_saveInterval);
            _saveManager.SaveCurrent();   
        }
    }
}

}