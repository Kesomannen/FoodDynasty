using TMPro;
using UnityEngine;

public class ItemCount : UIComponent<Item> {
    [SerializeField] TMP_Text _text;
    
    public override void SetContent(Item content) {
        _text.text = content.Count.ToString();
    }
}