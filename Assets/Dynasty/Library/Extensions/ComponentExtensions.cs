using System.Linq;
using UnityEngine;

namespace Dynasty.Library {

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
    
    public static T Error<T>(this Object component, string message) {
        Debug.LogError(message, component);
        return default;
    }
    
    public static void Error(this Object component, string message) {
        Debug.LogError(message, component);
    }
    
    public static void SetLayerRecursively(this GameObject gameObject, int layer) {
        foreach (var transform in gameObject.GetComponentsInChildren<Transform>()) {
            transform.gameObject.layer = layer;
        }
    }
}

}