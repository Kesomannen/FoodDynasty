using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemDataBuyControl : MonoBehaviour {
    [SerializeField] TMP_Text _costText;
    [SerializeField] string _costFormat = "Buy ({0})";
    [SerializeField] Button _buyButton;
    [Space]
    [SerializeField] Container<InventoryItemData> _container;
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

    public void Initialize(InventoryItemData content, MoneyManager moneyManager, Action<int> callback) {
        _callback = callback;
        _moneyManager = moneyManager;

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

    public void Buy() {
        _callback?.Invoke((int) _numberInput.Value);
    }
}