using UnityEngine;

namespace Dynasty.UI.Components {

public abstract class UIComponent<T> : MonoBehaviour {
    public abstract void SetContent(T content);
}

}