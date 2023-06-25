using UnityEngine;

public static class TransformExtensions {
    public static Vector3 GetLocalForward(this Transform transform) {
        return transform.localRotation * Vector3.forward;
    }
}