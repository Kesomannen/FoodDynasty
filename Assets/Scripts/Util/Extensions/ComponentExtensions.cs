using UnityEngine;

public static class ComponentExtensions {
    public static void SetActive(this Component component, bool active) {
        component.gameObject.SetActive(active);
    }
    
    public static T GetOrAddComponent<T>(this Component component) where T : Component {
        return component.gameObject.GetOrAddComponent<T>();
    }
    
    public static T GetOrAddComponent<T>(this GameObject component) where T : Component {
        return component.GetComponent<T>() ?? component.AddComponent<T>();
    }
}