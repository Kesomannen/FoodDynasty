using System;
using Dynasty.Core.Inventory;
using Dynasty.Library.Helpers;
using Dynasty.UI.Components;
using Dynasty.UI.Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dynasty.UI.Controls {

public class ItemDataBuyControl : MonoBehaviour {
    [SerializeField] TMP_Text _costText;
    [SerializeField] string _costFormat = "Buy ({0})";
    [SerializeField] Button _buyButton;
    [Space]
    [SerializeField] Container<ItemData> _container;
    [SerializeField] NumberInputController _numberInput;
    [SerializeField] NumberInputController.ModifyMode _modifyMode;

    Action<int> _callback;
    MoneyManager _moneyManager;

    void OnEnable() {
        _numberInput.OnValueChanged += OnValueChanged;
        _numberInput.OnSubmit += OnSubmit;
        _buyButton.onClick.AddListener(Buy);
    }
    
    void OnDisable() {
        _numberInput.OnValueChanged -= OnValueChanged;
        _numberInput.OnSubmit -= OnSubmit;
        _buyButton.onClick.RemoveListener(Buy);
    }

    void OnDestroy() {
        if (_moneyManager != null) {
            _moneyManager.OnMoneyChanged -= OnMoneyChanged;
        }
    }
    
    public void Initialize(ItemData content, MoneyManager moneyManager, Action<int> callback) {
        _callback = callback;

        if (_moneyManager != null) {
            _moneyManager.OnMoneyChanged -= OnMoneyChanged;
        }
        
        _moneyManager = moneyManager;
        _moneyManager.OnMoneyChanged += OnMoneyChanged;

        _numberInput.Initialize(
            startValue: 0,
            maxValue: GetMaxCount,
            mode: _modifyMode
        );

        _container.SetContent(content);
        UpdateElements(_numberInput.Value);
        
        float GetMaxCount() {
            var max = moneyManager.CurrentMoney / content.Price;
            max = Math.Floor(max);

            if (max > int.MaxValue) return int.MaxValue;
            return (int)max;
        }
    }

    void OnSubmit(float value) {
        Buy();
    }

    void OnValueChanged(float value) {
        if (_container.Content == null) return;
        UpdateElements(value);
    }

    void UpdateElements(float value) {
        var price = value * _container.Content.Price;
        _costText.text = string.Format(_costFormat, StringHelpers.FormatMoney(price));
        _buyButton.interactable = _moneyManager.CurrentMoney >= price;
    }

    void OnMoneyChanged(double prev, double current) {
        UpdateElements(_numberInput.Value);
    }

    void Buy() {
        _callback?.Invoke((int) _numberInput.Value);
    }
}

}