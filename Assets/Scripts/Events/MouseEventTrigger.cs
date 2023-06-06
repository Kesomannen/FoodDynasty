using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseEventTrigger<T> : MonoBehaviour, IPointerClickHandler {
    [SerializeField] protected T EventData;
    [SerializeField] Optional<GameEvent<T>> _clickGameEvent;
    [SerializeField] PointerEventData.InputButton[] _allowedButtons = { PointerEventData.InputButton.Left };
    
    public void OnPointerClick(PointerEventData eventData) {
        if (!_allowedButtons.Contains(eventData.button)) {
            return;
        }
        
        if (_clickGameEvent.Enabled) {
            _clickGameEvent.Value.Raise(EventData);
        }
    }
}