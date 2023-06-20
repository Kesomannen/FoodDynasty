using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class FadeTweenData : UITweenData {
    [Header("Fade")]
    [SerializeField] Mode _mode;

    protected override IEnumerable<LTDescr> GetTweens(RectTransform rectTransform) {
        var graphics = rectTransform.GetComponentsInChildren<Graphic>();
        
        var (from, to) = _mode switch {
            Mode.FadeIn => (0f, 1f),
            Mode.FadeOut => (1f, 0f),
            _ => throw new ArgumentOutOfRangeException()
        };

        return new[] {
            LeanTween.value(rectTransform.gameObject, from, to, Duration)
                .setOnUpdate(value => {
                    foreach (var graphic in graphics) {
                        var color = graphic.color;
                        color.a = value;
                        graphic.color = color;
                    }
                })
        };
    }

    enum Mode {
        FadeIn,
        FadeOut
    }
}