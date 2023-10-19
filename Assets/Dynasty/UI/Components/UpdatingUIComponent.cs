namespace Dynasty.UI.Components {

public abstract class UpdatingUIComponent<T> : UIComponent<T> {
    protected T Content { get; private set; }
    bool _isSubscribed;
    
    public sealed override void SetContent(T content) {
        if (_isSubscribed && Content != null) {
            Unsubscribe(Content);
        }

        Content = content;
        Subscribe(Content);
        _isSubscribed = true;
    }

    protected virtual void OnDisable() {
        if (!_isSubscribed || Content == null) return;
        Unsubscribe(Content);
        _isSubscribed = false;
    }

    protected abstract void Subscribe(T content);
    protected abstract void Unsubscribe(T content);
}

}