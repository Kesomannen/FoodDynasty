using System;
using Dynasty.Library.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dynasty.UI.Miscellaneous {

public class SceneLoader : MonoBehaviour {
    [SerializeField] GameEvent<int> _loadSceneEvent;

    void OnEnable() {
        _loadSceneEvent.AddListener(LoadScene);
    }
    
    void OnDisable() {
        _loadSceneEvent.RemoveListener(LoadScene);
    }

    static void LoadScene(int sceneId) {
        SceneManager.LoadScene(sceneId);
    }
}

}