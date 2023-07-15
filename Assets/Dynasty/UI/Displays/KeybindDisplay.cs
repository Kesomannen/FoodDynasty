using Dynasty.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Dynasty.UI.Displays {

public class KeybindDisplay : Container<IKeybind> {
    [Space]
    [SerializeField] TMP_Text _nameText;
    [SerializeField] TMP_Text _keyText;
    
    public override void SetContent(IKeybind content) {
        base.SetContent(content);
        
        _nameText.text = content.Name;
        _keyText.text = content.Action.GetBindingDisplayString();
    }
}

}