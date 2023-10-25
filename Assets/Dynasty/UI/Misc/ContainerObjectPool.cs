using Dynasty.Library;
using Dynasty.UI.Components;
using GenericUnityObjects;
using UnityEngine;

namespace Dynasty.UI.Miscellaneous {

[CreateGenericAssetMenu(MenuName = "Pooling/Container")]
public class ContainerObjectPool<T> : UIObjectPool<Container<T>> {
    public Container<T> Get(T content, Transform parent, int siblingIndex = 0) {
        var container = Get(parent);
        
        var containerTransform = container.transform;
        if (containerTransform.GetSiblingIndex() != siblingIndex) {
            containerTransform.SetSiblingIndex(siblingIndex);
        }
        
        container.SetContent(content);
        container.gameObject.SetActive(true);
        
        return container;
    }
}

}