using Dynasty.Machines;
using TMPro;
using UnityEngine;

namespace Dynasty.UI.Components {

public class AutoBuyerOverbuyAmount : UIComponent<AutoBuyer> {
    [SerializeField] TMP_InputField _inputField;
    
    AutoBuyer _autoBuyer;
    
    public override void SetContent(AutoBuyer content) {
        _autoBuyer = content;
        _inputField.text = _autoBuyer.OverbuyAmount.ToString();
        _inputField.onValueChanged.AddListener(OnValueChanged);
    }

    void OnValueChanged(string text) {
        if (int.TryParse(text, out var value)) {
            _autoBuyer.OverbuyAmount = value;
        }
    }
}

}