using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions {
    public static Vector3 GetLocalForward(this Transform transform) {
        return transform.localRotation * Vector3.forward;
    }
    
    public static IEnumerable<Transform> GetChildren(this Transform transform) {
        for (var i = 0; i < transform.childCount; i++) {
            yield return transform.GetChild(i);
        }
    }
}