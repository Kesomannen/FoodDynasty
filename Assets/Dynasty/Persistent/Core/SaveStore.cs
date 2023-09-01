using System;
using NaughtyAttributes;
using UnityEngine;

namespace Dynasty.Persistent.Core {

public abstract class SaveStore<T> : MonoBehaviour {
    [SerializeField] string _saveKey;
    [SerializeField] SaveManager _saveManager;
    [ReadOnly] [AllowNesting]
    [SerializeField] string _guid = Guid.NewGuid().ToString();
    
    protected abstract T GetDefaultData();
    protected abstract void OnAfterLoad(T saveData);
    protected abstract T GetSaveData();

    string SaveKey => $"{_saveKey}_{_guid}";

    protected virtual void Start() {
        OnAfterLoad(_saveManager.GetData(SaveKey, GetDefaultData()));
    }

    protected virtual void OnEnable() => _saveManager.OnSaveStarted += Save;
    protected virtual void OnDisable() => _saveManager.OnSaveStarted -= Save;
    void Save() => _saveManager.SetData(SaveKey, GetSaveData());
}

}