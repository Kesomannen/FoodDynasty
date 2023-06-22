using System;
using TMPro;
using UnityEngine;

public class InventoryItemDataBuyControl : MonoBehaviour {
    [SerializeField] TMP_Text _costText;
    [SerializeField] string _costFormat = "Buy ({0})";
    [Space]
    [SerializeField] Container<InventoryItemData> _container;
    [SerializeField] NumberInputController _numberInput;
    [SerializeField] NumberInputController.ModifyMode _modifyMode;

    Action<int> _callback;

    void OnEnable() {
        _numberInput.OnValueChanged += OnValueChanged;
    }
    
    void OnDisable() {
        _numberInput.OnValueChanged -= OnValueChanged;
    }

    void OnValueChanged(float value) {
        if (_container.Content == null) return;
        UpdateText(value);
    }

    void UpdateText(float value) {
        _costText.text = string.Format(_costFormat, StringHelpers.FormatMoney(value * _container.Content.Price));
    }

    public void Initialize(InventoryItemData content, MoneyManager moneyManager, Action<int> callback) {
        _callback = callback;

        _numberInput.Initialize(
            startValue: 1,
            maxValue: () => {
                var max = moneyManager.CurrentMoney / content.Price;
                max = Math.Floor(max);
                
                if (max > int.MaxValue) return int.MaxValue;
                return (int) max;
            },
            mode: _modifyMode
        );

        _container.SetContent(content);
        UpdateText(_numberInput.Value);
    }

    public void Buy() {
        _callback?.Invoke((int) _numberInput.Value);
    }
}