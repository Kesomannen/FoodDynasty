using System;
using System.Collections.Generic;
using Dynasty.Library.Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Dynasty.Library {

[CreateAssetMenu(menuName = "Manager/Input")]
public class InputManager : MonoScriptable {
    [SerializeField] ActionData[] _actions;
    [SerializeField] InputActionAsset _inputActionAsset;
    [SerializeField] string _inputActionMapName;

    readonly Dictionary<InputAction, GameEvent<InputAction.CallbackContext>> _boundActions = new();

    public override void OnAwake() {
        EnableActions();
    }

    public override void OnDestroy() {
        DisableActions();
    }
    
    void EnableActions() {
        _boundActions.Clear();
        foreach (var action in _actions) {
            if (!AssertHasEvent(action)) continue;
            if (!GetInputAction(action, out var inputAction)) continue;

            inputAction.performed += action.Event.Raise;
            inputAction.Enable();

            _boundActions.Add(inputAction, action.Event);
        }
    }

    void DisableActions() {
        foreach (var (inputAction, gameEvent) in _boundActions) {
            inputAction.Disable();
            inputAction.performed -= gameEvent.Raise;
        }

        _boundActions.Clear();
    }
    
    bool AssertHasEvent(ActionData action) {
        if (action.Event != null) return true;
        Debug.LogError($"Input action {action.InputActionName} has no event", this);
        return false;
    }

    bool GetInputAction(ActionData action, out InputAction inputAction) {
        inputAction = _inputActionAsset.FindAction($"{_inputActionMapName}/{action.InputActionName}");
        if (inputAction != null) return true;
        
        Debug.LogWarning($"Input action {action.InputActionName} not found", this);
        return false;
    }

    [Serializable]
    class ActionData {
        public string InputActionName;
        public GameEvent<InputAction.CallbackContext> Event;
    }
}

}