using UnityEngine;

public static class ComponentExtensions {
    public static void SetActive(this Component component, bool active) {
        component.gameObject.SetActive(active);
    }
}