using TMPro;
using UnityEngine;

public class InventoryItemDataPrice : UIComponent<InventoryItemData> {
    [SerializeField] TMP_Text _text;
    
    public override void SetContent(InventoryItemData content) {
        _text.text = StringUtil.FormatMoney(content.Price);
    }
}