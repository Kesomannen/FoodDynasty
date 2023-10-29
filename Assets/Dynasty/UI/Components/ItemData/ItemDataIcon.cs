using Dynasty;
using Google.MaterialDesign.Icons;
using UnityEngine;

namespace Dynasty.UI.Components {

public class ItemDataIcon : UIComponent<ItemData> {
    [SerializeField] MaterialIcon _icon;
    
    public override void SetContent(ItemData content) {
        ItemUIUtil.SetFromItemType(_icon, content.Type);
    }
}

}