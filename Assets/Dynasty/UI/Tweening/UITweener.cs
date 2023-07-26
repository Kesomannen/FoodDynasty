using System.Collections.Generic;
using UnityEngine;

namespace Dynasty.UI.Tweening {

[RequireComponent(typeof(RectTransform))]
public class UITweener : MonoBehaviour {
    [SerializeField] UITweenProfile _profile;

    RectTransform _rectTransform;

    void Awake() {
        _rectTransform = GetComponent<RectTransform>();
    }

    void OnEnable() {
        PlayTweens(_profile.EnableTweens);
    }

    void PlayTweens(IEnumerable<TweenData> tweens) {
        foreach (var tweenData in tweens) {
            tweenData.Setup(_rectTransform);
            tweenData.Play(_rectTransform);
        }
    }
}

}