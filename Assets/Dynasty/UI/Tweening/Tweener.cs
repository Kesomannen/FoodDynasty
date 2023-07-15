using System.Collections.Generic;
using UnityEngine;

namespace Dynasty.UI.Tweening {

[RequireComponent(typeof(RectTransform))]
public class Tweener : MonoBehaviour {
    [SerializeField] TweenProfile _profile;

    RectTransform _rectTransform;

    void Awake() {
        _rectTransform = GetComponent<RectTransform>();
    }

    void OnEnable() {
        PlayEnableTweens();
    }

    public void PlayEnableTweens() {
        PlayTweens(_profile.EnableTweens, true);
    }

    void PlayTweens(IEnumerable<TweenData> tweens, bool automatic) {
        foreach (var tweenData in tweens) {
            if (automatic && !tweenData.PlayAutomatically) continue;
            tweenData.Setup(_rectTransform);
            tweenData.Play(_rectTransform);
        }
    }
}

}