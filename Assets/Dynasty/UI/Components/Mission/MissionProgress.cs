using System;
using Dynasty.Library;
using Dynasty.UI.Tutorial;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dynasty.UI.Components {

public class MissionProgress : UpdatingUIComponent<Mission> {
    [SerializeField] TMP_Text _text;
    [SerializeField] Slider _bar;
    [SerializeField] Graphic _barFill;
    [Space]
    [SerializeField] float _tweenDuration;
    [SerializeField] LeanTweenType _tweenType;
    
    Color _originalColor;

    void Awake() {
        _originalColor = _barFill.color;
    }

    protected override void Subscribe(Mission content) {
        content.OnProgressChanged += OnProgressChanged;
        content.OnCompleted += OnCompleted;
        
        _bar.maxValue = content.Goal;
        _barFill.color = _originalColor;
        OnProgressChanged(content, content.Progress);
    }

    protected override void Unsubscribe(Mission content) {
        content.OnProgressChanged -= OnProgressChanged;
        content.OnCompleted -= OnCompleted;
    }
    
    void OnProgressChanged(Mission mission, float progress) {
        _text.text = mission.ProgressText;

        LeanTween.cancel(_bar.gameObject);
        LeanTween.value(_bar.gameObject, _bar.value, progress, _tweenDuration)
            .setOnUpdate(value => _bar.value = value)
            .setEase(_tweenType);
    }
    
    void OnCompleted(Mission mission) {
        LeanTween.color(_bar.fillRect, Colors.Positive, _tweenDuration)
            .setEase(_tweenType);
    }
}

}