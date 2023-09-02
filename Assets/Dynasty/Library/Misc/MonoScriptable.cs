using UnityEngine;

namespace Dynasty.Library {

public abstract class MonoScriptable : ScriptableObject {
    public virtual void OnAwake() { }
    public virtual void OnDestroy() { }
}

}