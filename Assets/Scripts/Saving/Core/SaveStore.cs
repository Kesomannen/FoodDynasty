using System;
using UnityEngine;

public abstract class SaveStore<T> : MonoBehaviour {
    [SerializeField] string _saveKey;
    [SerializeField] SaveManager _saveManager;
    
    protected abstract T DefaultValue { get; }
    protected abstract void OnAfterLoad(T saveData);
    protected abstract T GetSaveData();

    protected virtual void Start() {
        OnAfterLoad(_saveManager.LoadData(_saveKey, DefaultValue));
    }

    void OnEnable() {
        _saveManager.OnBeforeSave += Save;
    }
    
    void OnDisable() {
        _saveManager.OnBeforeSave -= Save;
    }

    void Save() {
        _saveManager.SaveData(_saveKey, GetSaveData());
    }
}