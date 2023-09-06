using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Dynasty.Persistent.Core {

public abstract class SaveStore<T> : MonoBehaviour {
    [SerializeField] string _saveKey;
    [SerializeField] SaveManager _saveManager;
    [SerializeField] bool _unique;
    [ReadOnly] [HideIf("_unique")] [AllowNesting]
    [SerializeField] string _guid = Guid.NewGuid().ToString();
    
    protected abstract T GetDefaultData();
    protected abstract void OnAfterLoad(T saveData);
    protected abstract T GetSaveData();

    string SaveKey => _unique ? _saveKey : $"{_saveKey}_{_guid}";

    protected virtual void Start() {
        OnAfterLoad(_saveManager.GetData(SaveKey, GetDefaultData()));
    }

    protected virtual void OnEnable() => _saveManager.OnSaveStarted += Save;
    protected virtual void OnDisable() => _saveManager.OnSaveStarted -= Save;
    void Save() => _saveManager.SetData(SaveKey, GetSaveData());
}

}