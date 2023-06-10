using Google.MaterialDesign.Icons;
using UnityEngine;

public class InventoryItemIcon : UIComponent<InventoryItemData> {
    [SerializeField] MaterialIcon _icon;
    
    public override void SetContent(InventoryItemData content) {
        _icon.SetFromItemType(content.Type);
    }
}