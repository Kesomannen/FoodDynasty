using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Interactable : Selectable, IDisposable, IPointerClickHandler {
    public event Action<Interactable, PointerEventData> OnClicked;
    public event Action<Interactable, bool, PointerEventData> OnHovered;
    
    public void OnPointerClick(PointerEventData eventData) {
        if (!IsInteractable()) return;
        OnClicked?.Invoke(this, eventData);
    }

    public override void OnPointerEnter(PointerEventData eventData) {
        base.OnPointerEnter(eventData);
        OnHovered?.Invoke(this, true, eventData);
    }

    public override void OnPointerExit(PointerEventData eventData) {
        base.OnPointerExit(eventData);
        OnHovered?.Invoke(this, false, eventData);
    }

    public void Dispose() {
        OnClicked = null;
        OnHovered = null;
    }
}