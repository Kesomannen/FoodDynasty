using TMPro;
using UnityEngine;

public class CurrentSupply : UpdatingUIComponent<SupplyBase> {
    [SerializeField] TMP_Text _text;

    protected override void Subscribe(SupplyBase content) {
        content.OnChanged += UpdateText;
        UpdateText(content);
    }

    protected override void Unsubscribe(SupplyBase content) {
        content.OnChanged -= UpdateText;
    }

    void UpdateText(SupplyBase content) {
        _text.text = content.CurrentSupply.ToString();
    }
}