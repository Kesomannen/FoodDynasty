using Dynasty.Library;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dynasty.UI.Miscellaneous {

public class UnderlayDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    [SerializeField] GenericGameEvent _clickEvent;
    
    public bool IsHovered { get; private set; }
    
    public void OnPointerEnter(PointerEventData eventData) {
        IsHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        IsHovered = false;
    }

    public void OnPointerClick(PointerEventData eventData) {
        _clickEvent.Raise();
    }
}

}