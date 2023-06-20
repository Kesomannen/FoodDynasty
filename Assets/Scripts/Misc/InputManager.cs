using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Manager/Input")]
public class InputManager : MonoScriptable {
    [SerializeField] ActionData[] _actions;
    [SerializeField] InputActionAsset _inputActionAsset;

    readonly Dictionary<InputAction, GameEvent<InputAction.CallbackContext>> _boundActions = new();

    public override void OnAwake() {
        EnableActions();
    }

    public override void OnDestroy() {
        DisableActions();
    }
    
    void EnableActions() {
        _boundActions.Clear();
        for (var i = 0; i < _actions.Length; i++) {
            var action = _actions[i];
            
            if (!AssertHasEvent(action)) continue;
            if (!GetInputAction(action, out var inputAction)) continue;

            inputAction.performed += action.Event.Raise;
            inputAction.Enable();

            _boundActions.Add(inputAction, action.Event);
            _actions[i] = action;
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
        inputAction = _inputActionAsset.FindAction(action.InputActionName);
        if (inputAction != null) return true;
        
        Debug.LogWarning($"Input action {action.InputActionName} not found", this);
        return false;
    }

    [Serializable]
    struct ActionData {
        public string InputActionName;
        public GameEvent<InputAction.CallbackContext> Event;
    }
}