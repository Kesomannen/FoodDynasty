using System;
using UnityEngine;

namespace Dynasty.Library {

public class MonoScriptableRunner : MonoBehaviour {
    [SerializeField] MonoScriptable[] _scriptableObjects;
    
    void Awake() => Invoke(obj => obj.OnAwake());
    void OnDestroy() => Invoke(obj => obj.OnDestroy());

    void Invoke(Action<MonoScriptable> action) {
        foreach (var scriptableObject in _scriptableObjects) {
            action(scriptableObject);
        }
    }
}

}