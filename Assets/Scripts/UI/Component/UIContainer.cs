using System;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIContainer<T> : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] bool _autoDetectComponents;
    [HideIf("_autoDetectComponents")]
    [SerializeField] UIComponent<T>[] _components;
    [SerializeField] bool _interactable = true;
    [SerializeField] GameObject _unInteractableOverlay;
    [SerializeField] PointerEventData.InputButton[] _allowedButtons = { PointerEventData.InputButton.Left };

    public event Action<UIContainer<T>> OnClicked;
    public event Action<UIContainer<T>, bool> OnHovered;

    public T Content { get; private set; }

    public bool Interactable {
        get => _interactable;
        set {
            _interactable = value;
            _unInteractableOverlay.SetActive(!value);
        }
    }

    void Awake() {
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
        if (!_interactable || !_allowedButtons.Contains(eventData.button)) return;
        OnClicked?.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (!_interactable) return;
        OnHovered?.Invoke(this, true);
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (!_interactable) return;
        OnHovered?.Invoke(this, false);
    }
}