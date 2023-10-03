using Dynasty.Library.Classes;
using Dynasty.Library;
using TMPro;
using UnityEngine;

namespace Dynasty.UI.Components {

public class EntityText<T> : UIComponent<T> where T : IEntityData {
    [SerializeField] TMP_Text _nameText;
    [SerializeField] string _nameFormat = "{0}";
    [SerializeField] Optional<TMP_Text> _descriptionText;
    [SerializeField] bool _useShortDescription;

    public override void SetContent(T content) {
        _nameText.text = string.Format(_nameFormat, content.Name);
        
        if (_descriptionText.Enabled) {
            _descriptionText.Value.text = _useShortDescription ? content.ShortDescription : content.Description;
        }
    }
}

}