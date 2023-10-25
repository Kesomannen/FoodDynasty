using Dynasty.Library;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Dynasty.Library {

[CreateAssetMenu(menuName = "Event/Input")]
public class InputEvent : GameEvent<InputAction.CallbackContext> {
    [SerializeField] InputActionReference _action;
    [SerializeField] Optional<string> _nameOverride;

    public string Name => _nameOverride.Enabled ? _nameOverride.Value : Action.name;
    public InputAction Action => _action.action;
    public InputActionReference Reference => _action;
}

}