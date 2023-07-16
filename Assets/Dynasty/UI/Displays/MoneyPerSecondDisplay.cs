using System.Collections;
using Dynasty.Library.Events;
using Dynasty.Library.Helpers;
using TMPro;
using UnityEngine;

namespace Dynasty.UI.Displays {

public class MoneyPerSecondDisplay : MonoBehaviour {
    [SerializeField] bool _countNegative;
    [SerializeField] TMP_Text _text;
    [SerializeField] float _updateInterval = 1f;
    [SerializeField] string _format = "{0} per second";
    [SerializeField] ValueChangedEvent<double> _moneyChangedEvent;

    double _increaseSinceUpdate;
    
    void OnEnable() {
        _moneyChangedEvent.OnRaised += OnMoneyChanged;
        StartCoroutine(UpdateLoop());
    }
    
    void OnDisable() {
        _moneyChangedEvent.OnRaised -= OnMoneyChanged;
    }

    IEnumerator UpdateLoop() {
        while (enabled) {
            yield return CoroutineHelpers.Wait(_updateInterval);
            UpdateText();
        }
    }
    
    void OnMoneyChanged(double prev, double current) {
        if (!_countNegative && current < prev) return;
        _increaseSinceUpdate += current - prev;
    }

    void UpdateText() {
        var perSecond = _increaseSinceUpdate / _updateInterval;
        _text.text = string.Format(_format, StringHelpers.FormatMoney(perSecond));
        _increaseSinceUpdate = 0;
    }
}

}