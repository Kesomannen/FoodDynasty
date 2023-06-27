using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UITweener : MonoBehaviour {
    [SerializeField] UITweenProfile _profile;

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

    void PlayTweens(IEnumerable<UITweenData> tweens, bool automatic) {
        foreach (var tweenData in tweens) {
            if (automatic && !tweenData.PlayAutomatically) continue;
            tweenData.Setup(_rectTransform);
            tweenData.Play(_rectTransform);
        }
    }
}