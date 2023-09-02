using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dynasty.UI.Miscellanious {

public class SliderInputField : MonoBehaviour {
    [SerializeField] Slider _slider;
    [SerializeField] TMP_InputField _inputField;

    void OnEnable() {
        _slider.onValueChanged.AddListener(OnValueChanged);
        _inputField.onEndEdit.AddListener(OnEndEdit);
        
        OnValueChanged(_slider.value);
    }
    
    void OnDisable() {
        _slider.onValueChanged.RemoveListener(OnValueChanged);
        _inputField.onEndEdit.RemoveListener(OnEndEdit);
    }
    
    void OnValueChanged(float value) {
        _inputField.text = value.ToString("0");
    }
    
    void OnEndEdit(string value) {
        if (float.TryParse(value, out var floatValue)) {
            _slider.value = floatValue;
        }
    }
}

}