using System;
using TMPro;
using UnityEngine;

public class MoneyDeltaSpawner : MonoBehaviour {
    [SerializeField] float _animationDuration;
    [SerializeField] Vector2 _animationMovement;
    [SerializeField] LeanTweenType _animationEaseType;
    [Space]
    [SerializeField] TMP_Text _textPrefab;
    [SerializeField] ValueChangedEvent<double> _moneyChangedEvent;

    void OnEnable() {
        _moneyChangedEvent.OnRaised += OnMoneyChanged;
    }
    
    void OnDisable() {
        _moneyChangedEvent.OnRaised -= OnMoneyChanged;
    }

    void OnMoneyChanged(double prev, double current) {
        var delta = current - prev;
        if (delta <= 0) return;

        var animationText = Instantiate(_textPrefab, transform);
        animationText.text = $"+{StringHelpers.FormatMoney(delta)}";
        
        var rectTransform = animationText.transform as RectTransform;
        if (rectTransform == null) return;
        
        var startPosition = Vector2.zero;
        
        var startColor = animationText.color;
        var endColor = startColor;
        endColor.a = 0;

        LeanTween.value(animationText.gameObject, 0, 1f, _animationDuration)
            .setEase(_animationEaseType)
            .setOnUpdate(value => {
                animationText.color = Color.Lerp(startColor, endColor, value);
                rectTransform.anchoredPosition = Vector2.Lerp(startPosition, startPosition + _animationMovement, value);
            })
            .setOnComplete(() => {
                Destroy(animationText.gameObject);
            });
    }
}