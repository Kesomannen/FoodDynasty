namespace Dynasty.UI.Components {

public abstract class UpdatingUIComponent<T> : UIComponent<T> {
    T _content;
    bool _isSubscribed;
    
    public sealed override void SetContent(T content) {
        if (_isSubscribed && _content != null) {
            Unsubscribe(_content);
        }

        _content = content;
        Subscribe(_content);
        _isSubscribed = true;
    }

    protected virtual void OnDisable() {
        if (!_isSubscribed || _content == null) return;
        Unsubscribe(_content);
        _isSubscribed = false;
    }

    protected abstract void Subscribe(T content);
    protected abstract void Unsubscribe(T content);
}

}