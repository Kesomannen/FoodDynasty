using Google.MaterialDesign.Icons;
using UnityEngine;

public class InventoryItemDataIcon : UIComponent<ItemData> {
    [SerializeField] MaterialIcon _icon;
    
    public override void SetContent(ItemData content) {
        _icon.SetFromItemType(content.Type);
    }
}