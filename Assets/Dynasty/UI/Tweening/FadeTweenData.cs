using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Dynasty.UI.Tweening {

[Serializable]
public class FadeTweenData : TweenData {
    [Header("Fade")] 
    [SerializeField] float _alpha;
    [SerializeField] Mode _mode;

    protected override IEnumerable<LTDescr> GetTweens(RectTransform rectTransform) {
        var (from, to) = _mode switch {
            Mode.FadeIn => (0f, _alpha),
            Mode.FadeOut => (_alpha, 0f),
            _ => throw new ArgumentOutOfRangeException()
        };

        return new[] {
            LeanTween.alpha(rectTransform, to, Duration).setFrom(from)
        };
    }

    enum Mode {
        FadeIn,
        FadeOut
    }
}

}