using System.Collections;
using Dynasty.Library;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dynasty.UI.Miscellaneous {

public class SceneLoader : MonoBehaviour {
    [SerializeField] CanvasGroup _overlay;
    [SerializeField] float _fadeDuration;
    [SerializeField] LeanTweenType _fadeEase;
    [SerializeField] GameEvent<int> _loadSceneEvent;

    static bool _flag;

    void Start() {
        if (!_flag) return;
        _flag = false;

        _overlay.alpha = 1;
        _overlay.SetActive(true);
        LeanTween.alphaCanvas(_overlay, 0, _fadeDuration)
            .setOnComplete(() => _overlay.SetActive(false))
            .setEase(_fadeEase);
    }

    void OnEnable() {
        _loadSceneEvent.AddListener(LoadScene);
    }
    
    void OnDisable() {
        _loadSceneEvent.RemoveListener(LoadScene);
    }

    void LoadScene(int sceneId) {
        _overlay.SetActive(true);
        _flag = true;
        
        _overlay.alpha = 0;
        LeanTween.alphaCanvas(_overlay, 1, _fadeDuration)
            .setIgnoreTimeScale(true)
            .setOnComplete(() => SceneManager.LoadScene(sceneId))
            .setEase(_fadeEase);
    }
}

}