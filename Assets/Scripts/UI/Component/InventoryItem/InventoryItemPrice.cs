using TMPro;
using UnityEngine;

public class InventoryItemPrice : UIComponent<InventoryItemData> {
    [SerializeField] TMP_Text _text;
    
    public override void SetContent(InventoryItemData content) {
        _text.text = StringUtil.FormatMoney(content.Price);
    }
}