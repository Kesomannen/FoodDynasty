using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dynasty.UI.Tweening {

[Serializable]
public class ScaleTweenData : TweenData {
    [Header("Scale")]
    [SerializeField] float _startScale;

    public override void Setup(RectTransform rectTransform) {
        rectTransform.localScale = Vector3.one * _startScale;
    }

    protected override IEnumerable<LTDescr> GetTweens(RectTransform rectTransform) {
        return new[] { LeanTween.scale(rectTransform, Vector3.one, Duration) };
    }
}

}