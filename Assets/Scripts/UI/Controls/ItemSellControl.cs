using System;
using Dynasty.Library.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSellControl : MonoBehaviour {
    [SerializeField] TMP_Text _moneyText;
    [SerializeField] string _moneyFormat = "Sell ({0})";
    [SerializeField] Button _sellButton;
    [Space]
    [SerializeField] Container<Item> _container;
    [SerializeField] NumberInputController _numberInput;
    [SerializeField] NumberInputController.ModifyMode _modifyMode;

    Action<int> _callback;
    InventoryAsset _inventory;
    ItemData _currentData;

    void OnEnable() {
        _numberInput.OnValueChanged += OnValueChanged;
        _numberInput.OnSubmit += OnSubmit;
        _sellButton.onClick.AddListener(Sell);
    }
    
    void OnDisable() {
        _numberInput.OnValueChanged -= OnValueChanged;
        _numberInput.OnSubmit -= OnSubmit;
        _sellButton.onClick.RemoveListener(Sell);
    }

    void OnDestroy() {
        if (_inventory != null) {
            _inventory.OnItemChanged -= OnItemChanged;
        }
    }
    
    public void Initialize(Item item, Action<int> callback) {
        _callback = callback;
        _currentData = item.Data;

        if (_inventory != null) {
            _inventory.OnItemChanged -= OnItemChanged;
        }
        
        _inventory = item.Inventory;
        _inventory.OnItemChanged += OnItemChanged;

        _numberInput.Initialize(
            startValue: 1,
            maxValue: GetMaxCount,
            mode: _modifyMode
        );

        _container.SetContent(item);
        UpdateElements(_numberInput.Value);
        
        float GetMaxCount() {
            return _inventory.GetCount(_currentData);
        }
    }

    void OnSubmit(float value) {
        Sell();
    }

    void OnValueChanged(float value) {
        UpdateElements(value);
    }

    void UpdateElements(float value) {
        var price = value * _currentData.Price;
        _moneyText.text = string.Format(_moneyFormat, StringHelpers.FormatMoney(price));
        _sellButton.interactable = value <= _inventory.GetCount(_currentData);
    }

    void OnItemChanged(Item item) {
        if (item.Data != _currentData) return;
        UpdateElements(_numberInput.Value);
    }

    void Sell() {
        _callback?.Invoke((int) _numberInput.Value);
    }
}