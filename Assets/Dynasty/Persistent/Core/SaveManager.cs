using System;
using System.Collections.Generic;
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

    Dictionary<string, object> _state;

    public int CurrentSaveSlot => _currentSaveSlot;
    public IReadOnlyDictionary<string, object> State => _state;
    
    public event Action OnBeforeSave;

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
            }
            else {
                interpreter.OnSaveNotFound();
            }
        }
    }
    
    public async Task UpdateAndSaveState(int slotIndex) {
        UpdateState();
        await _loader.Save(_state, slotIndex);
    }
    
    void UpdateState() {
        OnBeforeSave?.Invoke();
        
        foreach (var interpreter in _interpreters) {
            _state[interpreter.SaveKey] = interpreter.OnBeforeSave();
        }
    }

    public async void DeleteSlot(int slotIndex) {
        await _loader.Delete(slotIndex);
    }
    
    async Task SetSaveSlot(int value) {
        if (_currentSaveSlot == value) return;

        if (_currentSaveSlot != -1) {
            await UpdateAndSaveState(_currentSaveSlot);
        }

        _currentSaveSlot = value;
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

}