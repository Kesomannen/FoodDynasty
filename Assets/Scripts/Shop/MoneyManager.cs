using System;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Manager/Money")]
public class MoneyManager : ScriptableObject {
    [SerializeField] GameEvent<double> _onMoneyChanged;
    [SerializeField] double _startingMoney;
    [ReadOnly] [SerializeField] double _currentMoney;

    public double CurrentMoney {
        get => _currentMoney;
        set {
            var clamped = Math.Max(value, 0);
            if (Math.Abs(clamped - _currentMoney) < 0.05f) return;
            var previous = _currentMoney;
            
            _currentMoney = clamped;
            _onMoneyChanged.Raise(_currentMoney);
            OnMoneyChanged?.Invoke(previous, _currentMoney);
        }
    }
    
    public event Action<double, double> OnMoneyChanged; 
    
    void OnEnable() {
        CurrentMoney = _startingMoney;
    }
}