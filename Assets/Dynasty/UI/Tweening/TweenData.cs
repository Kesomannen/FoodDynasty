using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dynasty.UI.Tweening {

[Serializable]
public abstract class TweenData {
    [Header("Settings")]
    [SerializeField] float _duration = 0.3f;
    [SerializeField] LeanTweenType _easeType = LeanTweenType.easeOutBack;

    protected float Duration => _duration;

    public void Play(RectTransform rectTransform) {
        var tweens = GetTweens(rectTransform);
        foreach (var tween in tweens) {
            tween.setEase(_easeType);
        }
    }
    
    public virtual void Setup(RectTransform rectTransform) { }
    protected abstract IEnumerable<LTDescr> GetTweens(RectTransform rectTransform);
}

}