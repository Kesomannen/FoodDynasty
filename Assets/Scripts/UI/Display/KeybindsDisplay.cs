using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeybindsDisplay : MonoBehaviour {
    [SerializeField] Transform _keybindParent;
    [SerializeField] ContainerObjectPool<IKeybind> _keybindPool;
    [SerializeField] InputManager _inputManager;

    readonly List<Container<IKeybind>> _currentKeybinds = new();

    public void Show(IKeybind keybind) {
        var container = _keybindPool.Get(keybind, _keybindParent);
        _currentKeybinds.Add(container);
    }
    
    public void Hide(IKeybind keybind) {
        var container = _currentKeybinds.FirstOrDefault(container => container.Content == keybind);
        if (container == null) return;
        
        _currentKeybinds.Remove(container);
        container.Dispose();
    }
}

public interface IKeybind {
    string Name { get; }
    InputAction Action { get; }
}

public class Keybind : IKeybind {
    public string Name { get; }
    public InputAction Action { get; }

    public Keybind(string name, InputAction action) {
        Name = name;
        Action = action;
    }
    
    public Keybind(InputAction action) {
        Name = action.name;
        Action = action;
    }
}