using TMPro;
using UnityEngine;

public class InventoryItemCount : UIComponent<InventoryItem> {
    [SerializeField] TMP_Text _text;
    
    public override void SetContent(InventoryItem content) {
        _text.text = content.Count.ToString();
    }
}