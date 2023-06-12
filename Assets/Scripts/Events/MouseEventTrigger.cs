using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseEventTrigger<T> : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] protected T EventData;
    [SerializeField] Optional<GameEvent<T>> _clickGameEvent;
    [SerializeField] Optional<GameEvent<T>> _rightClickGameEvent;
    [SerializeField] Optional<GameEvent<T>> _enterGameEvent;
    [SerializeField] Optional<GameEvent<T>> _exitGameEvent;

    public void OnPointerClick(PointerEventData eventData) {
        switch (eventData.button) {
            case PointerEventData.InputButton.Left when _clickGameEvent.Enabled:
                _clickGameEvent.Value.Raise(EventData); break;
            case PointerEventData.InputButton.Right when _rightClickGameEvent.Enabled:
                _rightClickGameEvent.Value.Raise(EventData); break;
            case PointerEventData.InputButton.Middle: break;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (_enterGameEvent.Enabled) {
            _enterGameEvent.Value.Raise(EventData);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (_exitGameEvent.Enabled) {
            _exitGameEvent.Value.Raise(EventData);
        }
    }
}