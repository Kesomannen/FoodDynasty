using Dynasty.Machines;
using TMPro;
using UnityEngine;

namespace Dynasty.UI.Components {

public class SupplyCurrent : UpdatingUIComponent<Supply> {
    [SerializeField] TMP_Text _text;

    protected override void Subscribe(Supply content) {
        content.OnChanged += UpdateText;
        UpdateText(content);
    }

    protected override void Unsubscribe(Supply content) {
        content.OnChanged -= UpdateText;
    }

    void UpdateText(Supply content) {
        _text.text = content.CurrentSupply.ToString();
    }
}

}