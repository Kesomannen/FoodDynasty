using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseEventTrigger<T> : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] protected T EventData;
    [SerializeField] Optional<GameEvent<T>> _clickGameEvent;
    [SerializeField] Optional<GameEvent<T>> _enterGameEvent;
    [SerializeField] Optional<GameEvent<T>> _exitGameEvent;
    [SerializeField] PointerEventData.InputButton[] _allowedButtons = { PointerEventData.InputButton.Left };
    
    public void OnPointerClick(PointerEventData eventData) {
        if (!_allowedButtons.Contains(eventData.button)) {
            return;
        }
        
        if (_clickGameEvent.Enabled) {
            _clickGameEvent.Value.Raise(EventData);
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