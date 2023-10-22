using Dynasty.Machines;
using TMPro;
using UnityEngine;

namespace Dynasty.UI.Components {

public class RefillerStateComponent : UpdatingUIComponent<RefillerState> {
    [SerializeField] TMP_Text _itemText;
    [SerializeField] TMP_Text _statusText;
    [SerializeField] TMP_InputField _targetSupplyInput;

    void OnEnable() {
        _targetSupplyInput.onValueChanged.AddListener(OnTargetSupplyChanged);
    }
    
    protected override void OnDisable() {
        base.OnDisable();
        _targetSupplyInput.onValueChanged.RemoveListener(OnTargetSupplyChanged);
    }

    protected override void Subscribe(RefillerState content) {
        content.OnChanged += OnChanged;
        OnChanged(content);
    }

    protected override void Unsubscribe(RefillerState content) {
        content.OnChanged -= OnChanged;
    }
    
    void OnTargetSupplyChanged(string text) {
        if (int.TryParse(text, out var value)) {
            Content.TargetSupply = value;
        }
    }

    void OnChanged(RefillerState state) {
        _itemText.text = state.Supply.RefillItemName;
        _statusText.text = state.StatusString;
        _targetSupplyInput.text = state.TargetSupply.ToString();
    }
}

}