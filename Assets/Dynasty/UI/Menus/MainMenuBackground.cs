using System.Collections;
using System.Collections.Generic;
using Dynasty.Grid;
using Dynasty.Library;
using Dynasty.Machines;
using Dynasty.Persistent;
using UnityEngine;

namespace Dynasty.UI {

public class MainMenuBackground : MonoBehaviour {
    [SerializeField] MachineLoader _loader;
    [SerializeField] GridExpansionController _expansionController;
    [Space]
    [SerializeField] SaveManager _saveManager;
    [SerializeField] MainMenuPanorama _fallbackPanorama;
    [SerializeField] GridExpansionManager _expansionManager;
    [Space]
    [SerializeField] CanvasGroup _fadeGroup;
    [SerializeField] float _fadeDuration;
    [SerializeField] float _minimumBlackDuration;

    readonly List<(MachineSaveData, Vector2Int)> _saves = new();

    int _currentSlot = -1;
    
    IEnumerator Start() {
        var getSaves = _saveManager.SaveLoader.GetSaves().GetHandle();
        yield return getSaves;
        
        foreach (var saveSlot in getSaves.Result) {
            var getState = _saveManager.SaveLoader.Load(saveSlot.Id).GetHandle();
            yield return getState;
            
            if (!getState.Result.TryGetValue("machines", out var saveDataObj)) continue;
            if (!getState.Result.TryGetValue("expansion", out var expansionDataObj)) continue;
            
            var saveData = (MachineSaveData) saveDataObj;
            var expansionIndex = (int) expansionDataObj;
            
            _saves.Add((saveData, _expansionManager.GetSize(expansionIndex)));
        }

        if (_saves.Count == 0) {
            SetPanorama(_fallbackPanorama.SaveData, _fallbackPanorama.Size);
        } else {
            SetPanorama(Random.Range(0, _saves.Count));
        }
    }

    public void SetPanorama(int slot) {
        if (slot == _currentSlot) return;
        _currentSlot = slot;
        
        var (saveData, size) = _saves[slot];
        SetPanorama(saveData, size);
    }

    void SetPanorama(MachineSaveData saveData, Vector2Int size) {
        var alpha = _fadeGroup.alpha;
        var loadTime = 0f;
        
        LeanTween.cancel(_fadeGroup.gameObject);
        LeanTween.sequence()
            .append(LeanTween.alphaCanvas(_fadeGroup, 1, (1 - alpha) * _fadeDuration))
            .append(() => {
                var startTime = Time.realtimeSinceStartup;
                
                _loader.Clear();
                _expansionController.SetSize(size, true);
                _loader.Load(saveData);
            
                foreach (var supply in FindObjectsOfType<Supply>()) {
                    supply.CurrentSupply = int.MaxValue;
                }
                
                loadTime = Time.realtimeSinceStartup - startTime;
            })
            .append(Mathf.Max(_minimumBlackDuration - loadTime, 0))
            .append(LeanTween.alphaCanvas(_fadeGroup, 0, _fadeDuration));
    }
}

}