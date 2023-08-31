using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dynasty.Library;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dynasty.Persistent.Core {

[CreateAssetMenu(menuName = "Saving/Manager")]
public class SaveManager : MonoScriptable {
    [SerializeField] int _currentSaveSlot;
    [SerializeField] SaveLoader _loader;
    [SerializeField] SaveInterpreter[] _interpreters;

    public int CurrentSaveSlot {
        get => _currentSaveSlot;
        set => _currentSaveSlot = value;
    }

    Dictionary<string, object> _state;

    public IReadOnlyDictionary<string, object> State => _state;

    public event Action OnSaveStarted, OnSaveCompleted;

    public override async void OnAwake() {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        await LoadStateFrom(CurrentSaveSlot);
    }

    public override async void OnDestroy() {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        await UpdateAndSaveState(CurrentSaveSlot);
    }
    
    void OnSceneUnloaded(Scene scene) {
        UpdateState();
    }

    async Task LoadStateFrom(int slotIndex) {
        _state = await _loader.Load(slotIndex);

        foreach (var interpreter in _interpreters) {
            if (_state.TryGetValue(interpreter.SaveKey, out var saveData)) {
                interpreter.OnAfterLoad(saveData);
            } else {
                interpreter.OnSaveNotFound();
            }
        }
    }

    public async Task Save() {
        await UpdateAndSaveState(CurrentSaveSlot);
    }
    
    async Task UpdateAndSaveState(int slotIndex) {
        UpdateState();
        await _loader.Save(_state, slotIndex);
        OnSaveCompleted?.Invoke();
    }
    
    void UpdateState() {
        OnSaveStarted?.Invoke();
        
        foreach (var interpreter in _interpreters) {
            _state[interpreter.SaveKey] = interpreter.OnBeforeSave();
        }
    }

    public async void DeleteSlot(int slotIndex) {
        await _loader.Delete(slotIndex);
    }
    
    public async Task<SaveSlot[]> GetSaveSlots() {
        return (await _loader.GetSaveSlots()).ToArray();
    }

    public void SaveData<T>(string key, T data) {
        _state[key] = data;
    }

    public T LoadData<T>(string key, T defaultData) {
        if (_state.TryGetValue(key, out var data)) {
            return (T) data;
        }

        return defaultData;
    }
}

public struct SaveSlot {
    public DateTime LastPlayed;
}

}