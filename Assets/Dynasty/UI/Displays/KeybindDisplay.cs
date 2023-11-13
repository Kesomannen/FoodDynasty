using Dynasty.Library;
using Dynasty.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Dynasty.UI.Displays {

public class KeybindDisplay : Container<InputEvent>, IPointerClickHandler {
    [Space]
    [SerializeField] TMP_Text _nameText;
    [SerializeField] TMP_Text _keyText;
    
    public override void SetContent(InputEvent content) {
        base.SetContent(content);
        
        _nameText.text = content.Name;
        _keyText.text = content.Action.GetBindingDisplayString();
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        Content.Raise();
    }
}

}