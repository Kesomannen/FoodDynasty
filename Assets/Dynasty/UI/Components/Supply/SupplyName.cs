using Dynasty.Machines;
using TMPro;
using UnityEngine;

namespace Dynasty.UI.Components {

public class SupplyName : UIComponent<Supply> {
    [SerializeField] TMP_Text _text;
    [SerializeField] string _format = "{0} supply";
    
    public override void SetContent(Supply content) {
        _text.text = string.Format(_format, content.RefillItemName);
    }
}

}