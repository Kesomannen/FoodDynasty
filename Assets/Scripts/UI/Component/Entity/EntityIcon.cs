using UnityEngine;
using UnityEngine.UI;

public class EntityIcon<T> : UIComponent<T> where T : IEntityData {
    [SerializeField] Image _image;
    
    public override void SetContent(T content) {
        _image.sprite = content.Icon;
        _image.enabled = content.Icon != null;
    }
}