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
        foreach (var action in _actions) {
            if (action.Event == null) {
                Debug.LogWarning($"Input action {action.InputActionName} has no event", this);
                continue;
            }
            
            var inputAction = _inputActionAsset.FindAction(action.InputActionName);
            if (inputAction == null) {
                Debug.LogWarning($"Input action {action.InputActionName} not found", this);
                continue;
            }
            
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

    [Serializable]
    struct ActionData {
        public string InputActionName;
        public GameEvent<InputAction.CallbackContext> Event;
    }
}