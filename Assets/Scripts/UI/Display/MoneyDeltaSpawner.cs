using System;
using TMPro;
using UnityEngine;

public class MoneyDeltaSpawner : MonoBehaviour {
    [SerializeField] float _animationDuration;
    [SerializeField] Vector2 _animationMovement;
    [SerializeField] LeanTweenType _animationEaseType;
    [Space]
    [SerializeField] TMP_Text _textPrefab;
    [SerializeField] MoneyManager _moneyManager;

    void OnEnable() {
        _moneyManager.OnMoneyChanged += OnMoneyChanged;
    }
    
    void OnDisable() {
        _moneyManager.OnMoneyChanged -= OnMoneyChanged;
    }

    void OnMoneyChanged(double prev, double current) {
        var delta = current - prev;
        if (delta <= 0) return;

        var animationText = Instantiate(_textPrefab, transform);
        animationText.text = $"+{StringHelpers.FormatMoney(delta)}";
        
        var rectTransform = animationText.transform as RectTransform;
        if (rectTransform == null) return;
        
        var startColor = animationText.color;
        var startPosition = Vector2.zero;

        LeanTween.value(animationText.gameObject, 0, 1f, _animationDuration)
            .setEase(_animationEaseType)
            .setOnUpdate(value => {
                animationText.color = Color.Lerp(startColor, Color.clear, value);
                rectTransform.anchoredPosition = Vector2.Lerp(startPosition, startPosition + _animationMovement, value);
            })
            .setOnComplete(() => {
                Destroy(animationText.gameObject);
            });
    }
}