using System;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Manager/Money")]
public class MoneyManager : ScriptableObject {
    [SerializeField] double _startingMoney;
    [SerializeField] ValueChangedEvent<double> _moneyChangedEvent;

    double _currentMoney;

    public double CurrentMoney {
        get => _currentMoney;
        set {
            var clamped = Math.Max(value, 0);
            if (Math.Abs(clamped - _currentMoney) < 0.05f) return;
            var previous = _currentMoney;
            
            _currentMoney = clamped;
            _moneyChangedEvent.Raise(previous, _currentMoney);
            OnMoneyChanged?.Invoke(previous, _currentMoney);
        }
    }
    
    public event Action<double, double> OnMoneyChanged; 
    
    void OnEnable() {
        CurrentMoney = _startingMoney;
    }
}