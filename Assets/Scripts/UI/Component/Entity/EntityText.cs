using TMPro;
using UnityEngine;

public class EntityText<T> : UIComponent<T> where T : IEntityData {
    [SerializeField] TMP_Text _nameText;
    [SerializeField] Optional<TMP_Text> _descriptionText;
    
    public override void SetContent(T content) {
        _nameText.text = content.Name;
        if (_descriptionText.Enabled) {
            _descriptionText.Value.text = content.Description;
        }
    }
}