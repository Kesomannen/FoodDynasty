using Dynasty.Library.Events;
using Dynasty.Library.Helpers;
using Dynasty.Library.Pooling;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dynasty.UI.Displays {

public class MoneyDeltaSpawner : MonoBehaviour {
    [SerializeField] float _animationDuration;
    [SerializeField] Vector2 _animationMovement;
    [SerializeField] LeanTweenType _animationEaseType;
    [SerializeField] Color _startColor;
    [Space]
    [SerializeField] UIObjectPool<PoolableComponent<TextMeshProUGUI>> _textPool;
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

        var poolable = _textPool.Get(transform);
        var animationText = poolable.Component;
        
        animationText.text = $"+{StringHelpers.FormatMoney(delta)}";
        animationText.color = _startColor;

        var rectTransform = animationText.transform as RectTransform;
        if (rectTransform == null) return;
        
        var startPosition = Vector2.zero;
        
        var endColor = _startColor;
        endColor.a = 0;
        
        animationText.gameObject.SetActive(true);

        LeanTween.value(animationText.gameObject, 0, 1f, _animationDuration)
            .setEase(_animationEaseType)
            .setOnUpdate(value => {
                animationText.color = Color.Lerp(_startColor, endColor, value);
                rectTransform.anchoredPosition = Vector2.Lerp(startPosition, startPosition + _animationMovement, value);
            }).setOnComplete(poolable.Dispose);
    }
}

}