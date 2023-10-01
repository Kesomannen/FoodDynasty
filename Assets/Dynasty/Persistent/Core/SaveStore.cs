using System;
using NaughtyAttributes;
using UnityEngine;

namespace Dynasty.Persistent {

public abstract class SaveStore<T> : MonoBehaviour {
    [SerializeField] string _saveKey;
    [SerializeField] SaveManager _saveManager;
    [SerializeField] bool _unique;
    [ReadOnly] [HideIf("_unique")] [AllowNesting]
    [SerializeField] string _guid = Guid.NewGuid().ToString();
    
    protected abstract T DefaultData { get; }
    
    protected abstract void OnLoad(T saveData);
    protected abstract T GetSaveData();

    string SaveKey => _unique ? _saveKey : $"{_saveKey}_{_guid}";

    protected virtual void Start() {
        OnLoad(_saveManager.GetData(SaveKey, DefaultData));
    }

    protected virtual void OnEnable() => _saveManager.OnSaveStarted += Save;
    protected virtual void OnDisable() => _saveManager.OnSaveStarted -= Save;
    
    void Save() => _saveManager.SetData(SaveKey, GetSaveData());
}

}