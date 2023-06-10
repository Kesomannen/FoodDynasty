using System;
using NaughtyAttributes;
using UnityEngine;

public class Container<T> : MonoBehaviour, IPoolable<Container<T>> {
    [Header("Container")]
    [SerializeField] bool _autoDetectComponents;
    [HideIf("_autoDetectComponents")]
    [SerializeField] UIComponent<T>[] _components;

    public event Action<Container<T>> OnDisposed;
    public T Content { get; private set; }

    void Awake() {
        if (_autoDetectComponents) return;
        _components = GetComponentsInChildren<UIComponent<T>>();
    }

    public void SetContent(T content) {
        foreach (var component in _components) {
            component.SetContent(content);
        }

        Content = content;
    }

    public virtual void Dispose() {
        OnDisposed?.Invoke(this);
    }
}