using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Dynasty.UI.Controllers {

public class NumberInputController : MonoBehaviour {
    [Header("References")]
    [SerializeField] TMP_InputField _inputField;
    [SerializeField] NumberInputModifier _modifyButtonPrefab;
    [SerializeField] Transform _modifyButtonParent;
    
    [Header("Modes")]
    [SerializeField] float[] _constantValues;
    [SerializeField] float[] _percentageValues;
    
    Func<float> _maxValue;
    bool _integer;
    
    readonly List<NumberInputModifier> _modifyButtons = new();

    public float Value {
        get => string.IsNullOrEmpty(_inputField.text) ? 0 : float.Parse(_inputField.text);
        set => SetValue(value);
    }

    public event Action<float> OnSubmit;
    public event Action<float> OnValueChanged;

    void Start() {
        _inputField.onSubmit.AddListener(_ => OnSubmit?.Invoke(Value));
        _inputField.onValueChanged.AddListener(_ => OnValueChanged?.Invoke(Value));
    }

    public void Initialize(float startValue = 0, float maxValue = 0, bool integers = true, ModifyMode mode = ModifyMode.None) {
        Initialize(startValue, () => maxValue, integers, mode);
    }
    
    public void Initialize(float startValue = 0, Func<float> maxValue = null, bool integers = true, ModifyMode mode = ModifyMode.None) {
        maxValue ??= () => 0;
        
        _maxValue = maxValue;
        _integer = integers;
        
        _inputField.contentType = integers ? TMP_InputField.ContentType.IntegerNumber : TMP_InputField.ContentType.DecimalNumber;
        _inputField.characterValidation = integers ? TMP_InputField.CharacterValidation.Integer : TMP_InputField.CharacterValidation.Decimal;
        
        SetupButtons(mode);
        SetValue(startValue);
    }
    
    void SetupButtons(ModifyMode mode) {
        var buttons = 0;

        if (mode != ModifyMode.None) {
            var values = (mode == ModifyMode.Constant
                ? _constantValues.Select(v => (v, ModifyMode.Constant)).Prepend((1, ModifyMode.Percentage))
                : _percentageValues.Select(v => (v, ModifyMode.Percentage))).ToArray();
            
            buttons = values.Length;
            
            for (var i = 0; i < values.Length; i++) {
                NumberInputModifier button;

                if (_modifyButtons.Count > i) {
                    button = _modifyButtons[i];
                } else {
                    button = Instantiate(_modifyButtonPrefab, _modifyButtonParent);
                    _modifyButtons.Add(button);
                }

                button.SetContent(this, values[i].Item2, values[i].Item1);
            }
        }

        while (_modifyButtons.Count > buttons) {
            var lastIndex = _modifyButtons.Count - 1;
            Destroy(_modifyButtons[lastIndex].gameObject);
            _modifyButtons.RemoveAt(lastIndex);
        }
    }

    public void Add(float value) {
        SetValue(Value + value);
    }
    
    public void SetPercentage(float value) {
        SetValue(_maxValue() * value);
    }

    void SetValue(float value) {
        _inputField.text = _integer ? $"{Mathf.RoundToInt(value)}" : value.ToString(CultureInfo.CurrentCulture);
    }

    public enum ModifyMode {
        None,
        Constant,
        Percentage
    }
}

}