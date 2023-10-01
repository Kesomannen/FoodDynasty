using UnityEngine;

namespace Dynasty.Persistent {

public abstract class SaveInterpreter : ScriptableObject {
    [SerializeField] string _saveKey;

    public string SaveKey => _saveKey;

    public abstract void OnSaveNotFound();
    public abstract object OnBeforeSave();
    public abstract void OnLoad(object saveData);
}

public abstract class SaveInterpreter<T> : SaveInterpreter {
    protected abstract void OnLoad(T saveData);
    protected abstract T GetSaveData();

    protected virtual T DefaultData => default;

    public sealed override void OnSaveNotFound() => OnLoad(DefaultData);
    public sealed override object OnBeforeSave() => GetSaveData();
    public sealed override void OnLoad(object saveData) => OnLoad((T)saveData);
}

}