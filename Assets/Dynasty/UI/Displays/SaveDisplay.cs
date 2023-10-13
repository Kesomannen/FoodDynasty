using Dynasty.Persistent;
using UnityEngine;
using UnityEngine.UI;

namespace Dynasty.UI.Displays {

[RequireComponent(typeof(Graphic))]
public class SaveDisplay : MonoBehaviour {
    [SerializeField] float _fadeDuration = 0.5f;
    [SerializeField] SaveManager _saveManager;

    Graphic _graphic;
    
    void Awake() {
        _graphic = GetComponent<Graphic>();
        var color = _graphic.color;
        color.a = 0;
        _graphic.color = color;
    }

    void OnEnable() {
        _saveManager.OnSaveStarted += FadeIn;
        _saveManager.OnSaveCompleted += FadeOut;
    }
    
    void OnDisable() {
        _saveManager.OnSaveStarted -= FadeIn;
        _saveManager.OnSaveCompleted -= FadeOut;
    }
    
    void FadeIn() {
        LeanTween.cancel(gameObject);
        LeanTween.alpha(gameObject, 1, _fadeDuration);
    }
    
    void FadeOut() {
        LeanTween.cancel(gameObject);
        LeanTween.alpha(gameObject, 0, _fadeDuration).setDelay(1f);
    }
}

}