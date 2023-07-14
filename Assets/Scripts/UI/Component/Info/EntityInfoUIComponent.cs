using Dynasty.Library.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EntityInfoUIComponent : UIComponent<EntityInfo> {
    [SerializeField] TMP_Text _nameText;
    [SerializeField] TMP_Text _valueText;
    [SerializeField] Image _icon;
    
    public override void SetContent(EntityInfo content) {
        Configure(_nameText, content.Name);
        Configure(_valueText, content.Value);
        
        _icon.sprite = content.Icon;
        _icon.SetActive(content.Icon != null);

        void Configure(TMP_Text text, string value) {
            text.gameObject.SetActive(!string.IsNullOrEmpty(value));
            text.text = value;
        }
    }
}