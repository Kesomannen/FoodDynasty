using System.Collections.Generic;
using Dynasty.UI;
using Dynasty.Library;
using Dynasty.UI.Components;
using Dynasty.UI.Miscellaneous;
using UnityEngine;

namespace Dynasty.UI.Displays {

public class KeybindsDisplay : MonoBehaviour {
    [SerializeField] Transform _keybindParent;
    [SerializeField] ContainerObjectPool<InputEvent> _keybindPool;

    readonly Dictionary<InputEvent, Container<InputEvent>> _containers = new();

    void OnEnable() {
        Keybinds.OnKeybindActivated += Show;
        Keybinds.OnKeybindDeactivated += Hide;
    }
    
    void OnDisable() {
        Keybinds.OnKeybindActivated -= Show;
        Keybinds.OnKeybindDeactivated -= Hide;
    }

    void Show(InputEvent keybind) {
        var container = _keybindPool.Get(keybind, _keybindParent);
        _containers.Add(keybind, container);
    }
    
    void Hide(InputEvent keybind) {
        if (!_containers.TryGetValue(keybind, out var container)) return;
        
        _containers.Remove(keybind);
        container.Dispose();
    }
}

}