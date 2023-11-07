using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dynasty.Library;
using TMPro;
using UnityEngine;

namespace Dynasty.UI.Displays {

public class MoneyPerSecondDisplay : MonoBehaviour {
    [SerializeField] bool _countNegative;
    [SerializeField] TMP_Text _text;
    [SerializeField] float _updateInterval = 1f;
    [SerializeField] float _secondsToAverage = 5f;
    [SerializeField] string _format = "{0} per second";
    [SerializeField] ValueChangedEvent<double> _moneyChangedEvent;

    readonly Queue<double> _moneyChanges = new();
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
        _moneyChanges.Enqueue(perSecond);
        while (_moneyChanges.Count > _secondsToAverage / _updateInterval) {
            _moneyChanges.Dequeue();
        }
        var average = _moneyChanges.Average();
        _text.text = string.Format(_format, StringHelpers.FormatMoney(average));
        _increaseSinceUpdate = 0;
    }
}

}