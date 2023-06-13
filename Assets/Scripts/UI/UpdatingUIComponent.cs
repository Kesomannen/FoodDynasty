public abstract class UpdatingUIComponent<T> : UIComponent<T> {
    protected T Content { get; private set; }
    
    public sealed override void SetContent(T content) {
        if (Content != null) {
            Unsubscribe(Content);
        }
        
        Content = content;
        Subscribe(Content);
    }

    void OnDisable() {
        if (Content == null) return;
        Unsubscribe(Content);
        Content = default;
    }

    protected abstract void Subscribe(T content);
    protected abstract void Unsubscribe(T content);
}