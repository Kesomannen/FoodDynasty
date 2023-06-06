using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Manager/Money")]
public class MoneyManager : ScriptableObject {
    [SerializeField] GameEvent<float> _onMoneyChanged;
    
    float _currentMoney;

    public float CurrentMoney {
        get => _currentMoney;
        set {
            var clamped = Mathf.Max(value, 0);
            if (Mathf.Approximately(_currentMoney, clamped)) return;
            var previous = _currentMoney;
            _currentMoney = clamped;
            _onMoneyChanged.Raise(_currentMoney);
            OnMoneyChanged?.Invoke(previous, _currentMoney);
        }
    }
    
    public event Action<float, float> OnMoneyChanged; 
}