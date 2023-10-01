using Dynasty.Library.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dynasty.UI.Miscellaneous {

public class SceneLoader : MonoBehaviour {
    [SerializeField] CanvasGroup _overlay;
    [SerializeField] float _fadeDuration;
    [SerializeField] LeanTweenType _fadeEase;
    [SerializeField] GameEvent<int> _loadSceneEvent;

    static bool _flag;

    void Awake() {
        if (!_flag) return;
        
        _overlay.gameObject.SetActive(true);
        LeanTween.alphaCanvas(_overlay, 0, _fadeDuration)
            .setIgnoreTimeScale(true)
            .setOnComplete(() => {
                _flag = false;
                _overlay.gameObject.SetActive(false);
            })
            .setEase(_fadeEase)
            .setFrom(1);
    }

    void OnEnable() {
        _loadSceneEvent.AddListener(LoadScene);
    }
    
    void OnDisable() {
        _loadSceneEvent.RemoveListener(LoadScene);
    }

    void LoadScene(int sceneId) {
        _overlay.gameObject.SetActive(true);
        LeanTween.alphaCanvas(_overlay, 1, _fadeDuration)
            .setIgnoreTimeScale(true)
            .setOnComplete(() => {
                _flag = true;
                SceneManager.LoadScene(sceneId);
            })
            .setEase(_fadeEase);
    }
}

}