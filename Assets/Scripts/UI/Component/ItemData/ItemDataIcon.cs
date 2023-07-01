using Google.MaterialDesign.Icons;
using UnityEngine;

public class ItemDataIcon : UIComponent<ItemData> {
    [SerializeField] MaterialIcon _icon;
    
    public override void SetContent(ItemData content) {
        ItemUIUtil.SetFromItemType(_icon, content.Type);
    }
}