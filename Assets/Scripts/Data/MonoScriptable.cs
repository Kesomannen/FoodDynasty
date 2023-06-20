using UnityEngine;

public abstract class MonoScriptable : ScriptableObject {
    public virtual void OnAwake() { }
    public virtual void OnDestroy() { }
    public virtual void OnUpdate() { }
}