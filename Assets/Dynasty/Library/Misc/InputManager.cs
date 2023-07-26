using System.Collections.Generic;
using Dynasty.Library.Events;
using UnityEngine;

namespace Dynasty.Library {

[CreateAssetMenu(menuName = "Manager/Input")]
public class InputManager : MonoScriptable {
    [SerializeField] InputEvent[] _inputEvents;

    readonly List<InputEvent> _activatedEvents = new();

    public override void OnAwake() {
        EnableActions();
    }

    public override void OnDestroy() {
        DisableActions();
    }
    
    void EnableActions() {
        _activatedEvents.Clear();
        foreach (var inputEvent in _inputEvents) {
            inputEvent.Action.performed += inputEvent.Raise;
            inputEvent.Action.Enable();

            _activatedEvents.Add(inputEvent);
        }
    }

    void DisableActions() {
        foreach (var inputEvent in _activatedEvents) {
            inputEvent.Action.Disable();
            inputEvent.Action.performed -= inputEvent.Raise;
        }

        _activatedEvents.Clear();
    }
}

}