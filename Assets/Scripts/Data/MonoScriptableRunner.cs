using UnityEngine;

public class MonoScriptableRunner : MonoBehaviour {
    [SerializeField] MonoScriptable[] _scriptableObjects;
    
    void Awake() {
        foreach (var scriptableObject in _scriptableObjects) {
            scriptableObject.OnAwake();
        }
        DontDestroyOnLoad(gameObject);
    }

    void OnDestroy() {
        foreach (var scriptableObject in _scriptableObjects) {
            scriptableObject.OnDestroy();
        }
    }
}