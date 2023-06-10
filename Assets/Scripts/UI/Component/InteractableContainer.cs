using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractableContainer<T> : Selectable, IPointerClickHandler, IPoolable<InteractableContainer<T>> {
    [Header("Interactable Container")]
    [SerializeField] bool _autoDetectComponents;
    [HideIf("_autoDetectComponents")]
    [SerializeField] UIComponent<T>[] _components;
    
    public event Action<InteractableContainer<T>> OnDisposed;
    public event Action<InteractableContainer<T>, PointerEventData> OnClicked;
    public event Action<InteractableContainer<T>, bool, PointerEventData> OnHovered;

    public T Content { get; private set; }

    protected override void Awake() {
        if (_autoDetectComponents) return;
        _components = GetComponentsInChildren<UIComponent<T>>();
    }

    public virtual void SetContent(T content) {
        foreach (var component in _components) {
            component.SetContent(content);
        }

        Content = content;
    }

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
        
        OnDisposed?.Invoke(this);
    }
}