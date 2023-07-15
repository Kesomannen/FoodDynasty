using System;
using Dynasty.Library.Pooling;
using NaughtyAttributes;
using UnityEngine;

namespace Dynasty.UI.Components {

public class Container<T> : UIComponent<T>, IPoolable<Container<T>> {
    [Header("Container")]
    [SerializeField] bool _autoDetectComponents;
    [HideIf("_autoDetectComponents")]
    [SerializeField] UIComponent<T>[] _components;

    public event Action<Container<T>> OnDisposed;
    public T Content { get; private set; }

    void Awake() {
        if (!_autoDetectComponents) return;
        _components = GetComponentsInChildren<UIComponent<T>>();
    }

    public override void SetContent(T content) {
        foreach (var component in _components) {
            component.SetContent(content);
        }

        Content = content;
    }

    public void Dispose() {
        OnDisposed?.Invoke(this);
    }
}

}