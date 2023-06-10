using UnityEngine;
using UnityEngine.UI;

public class InventoryItemImage : UIComponent<InventoryItemData> {
    [SerializeField] Image _image;
    
    public override void SetContent(InventoryItemData content) {
        _image.sprite = content.Image;
    }
}