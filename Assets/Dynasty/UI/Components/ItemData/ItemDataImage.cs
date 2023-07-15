using Dynasty.Core.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace Dynasty.UI.Components {

public class ItemDataImage : UIComponent<ItemData> {
    [SerializeField] Image _image;
    
    public override void SetContent(ItemData content) {
        _image.sprite = content.Icon;
    }
}

}