using UnityEngine;

public abstract class UIComponent<T> : MonoBehaviour {
    public abstract void SetContent(T content);
}