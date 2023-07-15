using Dynasty.Library.Events;
using Dynasty.Library.Helpers;
using Dynasty.UI.Tooltip;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dynasty.UI.Miscellaneous {

public class DraggableWindow : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerClickHandler {
    [SerializeField] bool _closeOnClick;
    [SerializeField] bool _startAtMouse;
    [ShowIf("_startAtMouse")] [AllowNesting]
    [SerializeField] Vector2 _mouseOffset;
    [SerializeField] GenericGameEvent _closeEvent;
    
    RectTransform _rectTransform;
    Vector2 _offset;
    bool _isDragging;

    void Awake() {
        _rectTransform = GetComponent<RectTransform>();
    }
    
    void OnEnable() {
        _rectTransform.anchoredPosition = Vector2.zero;
        transform.SetAsLastSibling();
        _isDragging = false;
        
        if (_startAtMouse) {
            var mousePos = MouseHelpers.ScreenPosition + _mouseOffset;
            _rectTransform.PositionAsTooltip(mousePos);
        }
        
        _closeEvent.AddListener(OnClose);
    }

    void OnDisable() {
        _closeEvent.RemoveListener(OnClose);
    }

    void OnClose() {
        if (transform.GetSiblingIndex() == transform.parent.childCount - 1) {
            gameObject.SetActive(false);
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData) {
        _offset = eventData.position - (Vector2) transform.position;
        _isDragging = true;
    }

    public void OnDrag(PointerEventData eventData) {
        if (_isDragging) {
            transform.position = eventData.position - _offset;
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        _isDragging = false;
    }

    public void OnPointerDown(PointerEventData eventData) {
        transform.SetAsLastSibling();
    }
    
    public void OnPointerClick(PointerEventData eventData) {
        if (_closeOnClick) {
            gameObject.SetActive(false);
        }
    }
    
    public void Toggle() {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}

}