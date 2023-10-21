using UnityEngine;

public class ScaleTween : MonoBehaviour {
    [SerializeField] float _startScale;
    [SerializeField] float _duration;
    [SerializeField] LeanTweenType _easeType;

    void OnEnable() {
        LeanTween.scale(gameObject, Vector3.one, _duration)
            .setFrom(_startScale * Vector3.one)
            .setEase(_easeType);
    }
}