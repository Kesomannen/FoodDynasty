using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dynasty.Library;
using Dynasty.Machines;
using Dynasty.Persistent;
using Dynasty.Persistent.Mapping;
using UnityEngine;
using UnityEngine.UI;

namespace Dynasty.UI {

public class MainMenuBackground : MonoBehaviour {
    [SerializeField] float _duration;
    [SerializeField] MachineLoader _loader;
    [SerializeField] SaveManager _saveManager;
    [SerializeField] MainMenuPanorama _fallbackPanorama;
    [Space]
    [SerializeField] float _fadeDuration;
    [SerializeField] float _blackDuration;
    [SerializeField] RectTransform _overlay;

    IEnumerator Start() {
        var i = 0;
        var getSaves = _saveManager.SaveLoader.GetSaves().GetHandle();
        yield return getSaves;

        var saves = new List<MachineSaveData>();
        
        foreach (var saveSlot in getSaves.Result) {
            var getState = _saveManager.SaveLoader.Load(saveSlot.Id).GetHandle();
            yield return getState;
            
            if (!getState.Result.TryGetValue("machines", out var saveDataObj)) continue;
            var saveData = (MachineSaveData) saveDataObj;

            if (saveData.ItemIds.Length == 0) continue;
            saves.Add(saveData);
        }

        while (enabled) {
            MachineSaveData saveData;
            if (saves.Count == 0) {
                saveData = _fallbackPanorama.SaveData;
            } else {
                saveData = saves[i];
                i = (i + 1) % saves.Count;
            }
            
            _loader.Clear();
            _loader.Load(saveData);
            
            foreach (var supply in FindObjectsOfType<Supply>()) {
                supply.CurrentSupply = 10000;
            }
            
            LeanTween.alpha(_overlay, 0, _fadeDuration);
            yield return CoroutineHelpers.Wait(_duration + _fadeDuration);
            LeanTween.alpha(_overlay, 1, _fadeDuration);
            yield return CoroutineHelpers.Wait(_fadeDuration + _blackDuration);
        }
    }
}

}