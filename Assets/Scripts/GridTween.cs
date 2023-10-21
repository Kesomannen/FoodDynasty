using UnityEngine;

public class GridTween : MonoBehaviour {
    [SerializeField] float _moveDistance;
    [SerializeField] float _duration;
    [SerializeField] LeanTweenType _easeType;

    void Awake() {
        var position = transform.position;
        LeanTween.moveY(gameObject, position.y, _duration)
            .setFrom(position.y - _moveDistance)
            .setEase(_easeType);
    }
}