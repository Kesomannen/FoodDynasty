using System;
using TMPro;
using UnityEngine;

public class MoneyCounter : MonoBehaviour {
    [SerializeField] TMP_Text _moneyText;
    [SerializeField] MoneyManager _moneyManager;

    void OnEnable() {
        _moneyManager.OnMoneyChanged += OnMoneyChanged;
        OnMoneyChanged(0, _moneyManager.CurrentMoney);
    }
    
    void OnDisable() {
        _moneyManager.OnMoneyChanged -= OnMoneyChanged;
    }
    
    void OnMoneyChanged(float prev, float current) {
        _moneyText.text = $"${current:0}";
    }
}