using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Saving/Manager")]
public class SaveManager : MonoScriptable {
    [SerializeField] int _currentSaveSlot;
    [SerializeField] SaveLoader _loader;
    [SerializeField] SaveInterpreter[] _interpreters;

    Dictionary<string, object> _state;

    public int CurrentSaveSlot {
        get => _currentSaveSlot;
        set => _currentSaveSlot = value;
    }
    
    public IReadOnlyDictionary<string, object> State => _state;
    
    public event Action OnBeforeSave;

    public override async void OnAwake() {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        
        await LoadFromCurrent();
    }

    public override async void OnDestroy() {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        
        await SaveToCurrent();
    }

    async void OnSceneUnloaded(Scene scene) {
        await SaveToCurrent();
    }

    async Task LoadFromCurrent() {
        await Load(_currentSaveSlot);
    }
    
    async Task Load(int slotIndex) {
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
    
    public async Task SaveToCurrent() {
        await Save(_currentSaveSlot);
    }
    
    public async Task Save(int slotIndex) {
        OnBeforeSave?.Invoke();
        
        foreach (var interpreter in _interpreters) {
            _state[interpreter.SaveKey] = interpreter.OnBeforeSave();
        }

        await _loader.Save(_state, slotIndex);
    }
    
    public async void DeleteSlot(int slotIndex) {
        await _loader.Delete(slotIndex);
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