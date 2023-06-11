using TMPro;
using UnityEngine;

public class MoneyDisplay : MonoBehaviour {
    [SerializeField] TMP_Text _text;
    [SerializeField] MoneyManager _moneyManager;

    void OnEnable() {
        _moneyManager.OnMoneyChanged += OnMoneyChanged;
        UpdateText();
    }
    
    void OnDisable() {
        _moneyManager.OnMoneyChanged -= OnMoneyChanged;
    }

    void OnMoneyChanged(double prev, double current) {
        UpdateText();
    }

    void UpdateText() {
        _text.text = StringHelpers.FormatMoney(_moneyManager.CurrentMoney);
    }
}