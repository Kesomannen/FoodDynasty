using Google.MaterialDesign.Icons;
using UnityEngine;

public class InventoryItemDataIcon : UIComponent<InventoryItemData> {
    [SerializeField] MaterialIcon _icon;
    
    public override void SetContent(InventoryItemData content) {
        _icon.SetFromItemType(content.Type);
    }
}