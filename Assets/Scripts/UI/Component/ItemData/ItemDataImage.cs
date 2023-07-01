using UnityEngine;
using UnityEngine.UI;

public class ItemDataImage : UIComponent<ItemData> {
    [SerializeField] Image _image;
    
    public override void SetContent(ItemData content) {
        _image.sprite = content.Image;
    }
}