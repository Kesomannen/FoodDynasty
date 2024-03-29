﻿using Dynasty;
using Dynasty.Library;
using TMPro;
using UnityEngine;

namespace Dynasty.UI.Components {

public class ItemDataPrice : UIComponent<ItemData> {
    [SerializeField] TMP_Text _text;
    
    public override void SetContent(ItemData content) {
        _text.text = StringHelpers.FormatMoney(content.Price);
    }
}

}