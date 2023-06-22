using GenericUnityObjects;

[CreateGenericAssetMenu(MenuName = "Pooling/Container")]
public class ContainerObjectPool<T> : CustomObjectPool<Container<T>> {
    public Container<T> Get(T content) {
        var container = Get();
        container.SetContent(content);
        return container;
    }
}