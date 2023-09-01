using Dynasty.Library.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dynasty.UI.Miscellaneous {

public class SceneLoader : MonoBehaviour {
    [SerializeField] RectTransform _overlay;
    [SerializeField] float _fadeDuration;
    [SerializeField] LeanTweenType _fadeEase;
    [SerializeField] GameEvent<int> _loadSceneEvent;

    static bool _flag;

    void Awake() {
        if (!_flag) return;
        
        _overlay.gameObject.SetActive(false);
        LeanTween.alpha(_overlay, 0, _fadeDuration)
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
        LeanTween.alpha(_overlay, 1, _fadeDuration)
            .setOnComplete(() => {
                _flag = true;
                SceneManager.LoadScene(sceneId);
            })
            .setEase(_fadeEase);
    }
}

}