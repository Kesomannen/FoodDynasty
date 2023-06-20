using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class NumberInputModifier : MonoBehaviour {
    [SerializeField] TMP_Text _text;
    [SerializeField] Interactable _button;
    
    NumberInputController _controller;
    NumberInputController.ModifyMode _mode;
    float _value;
    
    public void SetContent(NumberInputController controller, NumberInputController.ModifyMode mode, float value) {
        if (_controller != null) {
            _button.OnClicked -= OnClicked;
        }
        
        _controller = controller;
        _mode = mode;
        _value = value;
        
        _button.OnClicked += OnClicked;
        _text.text = GetText();
    }
    
    void OnClicked(Interactable interactable, PointerEventData eventData) {
        switch (_mode) {
            case NumberInputController.ModifyMode.Constant:
                _controller.Add(_value); break;
            case NumberInputController.ModifyMode.Percentage:
                _controller.SetPercentage(1 / _value); break;
            case NumberInputController.ModifyMode.None:
            default: throw new ArgumentOutOfRangeException();
        }
    }

    string GetText() {
        switch (_mode) {
            case NumberInputController.ModifyMode.Constant:
                return $"{_value:0.##}";
            case NumberInputController.ModifyMode.Percentage:
                return Math.Abs(_value - 1) < 0.05f ? "Max" : $"{1 / _value:P0}";
            case NumberInputController.ModifyMode.None:
            default: throw new ArgumentOutOfRangeException();
        }
    }
}