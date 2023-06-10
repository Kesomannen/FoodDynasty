using UnityEngine;
using UnityEngine.UI;

public class InventoryItemDataImage : UIComponent<InventoryItemData> {
    [SerializeField] Image _image;
    
    public override void SetContent(InventoryItemData content) {
        _image.sprite = content.Image;
    }
}