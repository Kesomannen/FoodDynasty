using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Dynasty.UI.Tweening {

[Serializable]
public class FadeTweenData : TweenData {
    [Header("Fade")]
    [SerializeField] Mode _mode;

    protected override IEnumerable<LTDescr> GetTweens(RectTransform rectTransform) {
        var graphics = rectTransform.GetComponentsInChildren<Graphic>().ToList();
        
        var (from, to) = _mode switch {
            Mode.FadeIn => (0f, 1f),
            Mode.FadeOut => (1f, 0f),
            _ => throw new ArgumentOutOfRangeException()
        };

        return new[] {
            LeanTween.value(rectTransform.gameObject, from, to, Duration)
                .setOnUpdate(value => {
                    for (var i = 0; i < graphics.Count; i++) {
                        var graphic = graphics[i];

                        if (graphic == null) {
                            graphics.RemoveAt(i);
                            i--;
                            continue;
                        }
                        
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

}