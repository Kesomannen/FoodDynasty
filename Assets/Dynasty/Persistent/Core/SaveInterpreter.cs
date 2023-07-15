using UnityEngine;

namespace Dynasty.Persistent.Core {

public abstract class SaveInterpreter : ScriptableObject {
    [SerializeField] string _saveKey;

    public string SaveKey => _saveKey;

    public abstract void OnSaveNotFound();
    public abstract object OnBeforeSave();
    public abstract void OnAfterLoad(object saveData);
}

public abstract class SaveInterpreter<T> : SaveInterpreter {
    protected abstract void OnAfterLoad(T saveData);
    protected abstract T GetSaveData();

    protected virtual T DefaultValue => default;

    public sealed override void OnSaveNotFound() => OnAfterLoad(DefaultValue);
    public sealed override object OnBeforeSave() => GetSaveData();
    public sealed override void OnAfterLoad(object saveData) => OnAfterLoad((T)saveData);
}

}