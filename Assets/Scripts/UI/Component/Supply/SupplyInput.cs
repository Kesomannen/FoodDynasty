using System;
using TMPro;
using UnityEngine;

public class SupplyInput : UIComponent<SupplyBase> {
    [SerializeField] TMP_InputField _inputField;
    [SerializeField] InventoryAsset _inventory;
    [SerializeField] GameObject[] _hideIfNotRefillable;
    
    SupplyBase _content;
    
    public override void SetContent(SupplyBase content) {
        _content = content;
        foreach (var obj in _hideIfNotRefillable) {
            obj.SetActive(content.IsRefillable);
        }
    }

    void OnEnable() {
        _inputField.onValidateInput += OnValidateInput;
        _inputField.onSubmit.AddListener(Add);
    }
    
    void OnDisable() {
        _inputField.onValidateInput -= OnValidateInput;
        _inputField.onSubmit.RemoveListener(Add);
    }

    static char OnValidateInput(string text, int charIndex, char addedChar) {
        return char.IsDigit(addedChar) ? addedChar : '\0';
    }

    public void Add() {
        Add(_inputField.text);
    }

    public void Add(string text) {
        if (_content == null || !_content.IsRefillable) return;
        
        var inventoryCount = _inventory.GetCount(_content.RefillItem);
        var clampedInput = Mathf.Clamp(int.Parse(text), 0, inventoryCount);
        if (clampedInput == 0) return;
            
        if (!_inventory.Remove(_content.RefillItem, clampedInput)) return;
        _content.CurrentSupply += clampedInput;
        _inputField.text = _inventory.GetCount(_content.RefillItem).ToString();
    }
    
    public void Subtract() {
        Subtract(_inputField.text);
    }
    
    public void Subtract(string text) {
        if (_content == null || !_content.IsRefillable) return;
        
        var clampedInput = Mathf.Clamp(int.Parse(text), 0, _content.CurrentSupply);
        if (clampedInput == 0) return;
        
        _content.CurrentSupply -= clampedInput;
        _inventory.Add(_content.RefillItem, clampedInput);
        _inputField.text = _content.CurrentSupply.ToString();
    }
}