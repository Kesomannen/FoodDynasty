using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Dynasty.Persistent {

[CreateAssetMenu(menuName = "Saving/Manager")]
public class SaveManager : ScriptableObject {
    [SerializeField] SaveLoader _loader;
    [SerializeField] SaveInterpreter[] _interpreters;

    public int CurrentSaveId { get; set; }

    Dictionary<string, object> _state;

    public IReadOnlyDictionary<string, object> State => _state;

    public event Action OnSaveStarted, OnSaveCompleted;
    
    public async Task SaveCurrent() {
        await UpdateAndSaveState(CurrentSaveId);
    }
    
    public async Task LoadCurrent() {
        await LoadStateFrom(CurrentSaveId);
    }
    
    public async void DeleteSave(int saveId) {
        await _loader.Delete(saveId);
    }
    
    async Task LoadStateFrom(int saveId) {
        _state = await _loader.Load(saveId);

        foreach (var interpreter in _interpreters) {
            if (_state.TryGetValue(interpreter.SaveKey, out var saveData)) {
                interpreter.OnLoad(saveData);
            } else {
                interpreter.OnSaveNotFound();
            }
        }
    }
    
    async Task UpdateAndSaveState(int saveId) {
        UpdateState();
        await _loader.Save(_state, saveId);
        OnSaveCompleted?.Invoke();
    }
    
    void UpdateState() {
        OnSaveStarted?.Invoke();
        
        foreach (var interpreter in _interpreters) {
            _state[interpreter.SaveKey] = interpreter.OnBeforeSave();
        }
    }
    
    public async Task<SaveSlot[]> GetSaves() {
        return (await _loader.GetSaves()).ToArray();
    }

    public void SetData<T>(string key, T data) {
        _state[key] = data;
    }

    public T GetData<T>(string key, T defaultData) {
        if (_state.TryGetValue(key, out var data)) {
            return (T) data;
        }

        return defaultData;
    }
}

public struct SaveSlot {
    public int Id;
    public DateTime LastPlayed;
}

}