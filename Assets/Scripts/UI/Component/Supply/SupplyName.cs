using TMPro;
using UnityEngine;

public class SupplyName : UIComponent<SupplyBase> {
    [SerializeField] TMP_Text _text;
    [SerializeField] string _format = "{0} supply";
    
    public override void SetContent(SupplyBase content) {
        _text.text = string.Format(_format, content.RefillItemName);
    }
}