using GenericUnityObjects;
using UnityEngine;

[CreateGenericAssetMenu(MenuName = "Pooling/Container")]
public class ContainerObjectPool<T> : UIObjectPool<Container<T>> {
    public Container<T> Get(T content, Transform parent) {
        var container = Get(parent);
        container.SetContent(content);
        container.gameObject.SetActive(true);
        
        return container;
    }
}