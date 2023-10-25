using System;
using Dynasty.Library;

namespace Dynasty.Core.Tooltip {

public static class Keybinds {
    public static event Action<InputEvent> OnKeybindActivated;
    public static event Action<InputEvent> OnKeybindDeactivated;

    public static void Activate(params InputEvent[] keybinds) {
        foreach (var keybind in keybinds) {
            OnKeybindActivated?.Invoke(keybind);
        }
    }
    
    public static void Deactivate(params InputEvent[] keybinds) {
        foreach (var keybind in keybinds) {
            OnKeybindDeactivated?.Invoke(keybind);
        }
    }
}

}