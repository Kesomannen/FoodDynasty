using UnityEngine;

namespace Dynasty.Library {

public class MonoScriptableRunner : MonoBehaviour {
    [SerializeField] MonoScriptable[] _scriptableObjects;
    
    void Awake() {
        foreach (var scriptableObject in _scriptableObjects) {
            scriptableObject.OnAwake();
        }
    }

    void OnDestroy() {
        foreach (var scriptableObject in _scriptableObjects) {
            scriptableObject.OnDestroy();
        }
    }
}

}