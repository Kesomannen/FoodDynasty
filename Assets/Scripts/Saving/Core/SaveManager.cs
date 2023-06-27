using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

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
    public event Action<bool> OnAfterLoad;

    public override async void OnAwake() {
        await LoadCurrent(true);
    }

    public override async void OnDestroy() {
        await SaveCurrent();
    }

    public async Task LoadCurrent(bool isFirstLoad = false) {
        _state = await _loader.Load(_currentSaveSlot);

        if (isFirstLoad) {
            foreach (var interpreter in _interpreters) {
                if (_state.TryGetValue(interpreter.SaveKey, out var saveData)) {
                    interpreter.OnAfterLoad(saveData);
                }
                else {
                    interpreter.OnSaveNotFound();
                }
            }
        }

        OnAfterLoad?.Invoke(isFirstLoad);
    }
    
    public async Task SaveCurrent() {
        OnBeforeSave?.Invoke();
        
        foreach (var interpreter in _interpreters) {
            _state[interpreter.SaveKey] = interpreter.OnBeforeSave();
        }

        await _loader.Save(_state, _currentSaveSlot);
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