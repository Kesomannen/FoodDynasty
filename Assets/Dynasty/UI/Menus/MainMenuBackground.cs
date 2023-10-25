using System.Collections;
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

        var saves = getSaves.Result.ToArray();
        
        while (enabled) {
            MachineSaveData saveData;
            if (saves.Length == 0) {
                saveData = _fallbackPanorama.SaveData;
            } else {
                var getState = _saveManager.SaveLoader.Load(saves[i].Id).GetHandle();
                yield return getState;

                if (getState.Result.TryGetValue("machines", out var saveDataObj))
                    saveData = (MachineSaveData) saveDataObj;
                else
                    saveData = _fallbackPanorama.SaveData;

                i = (i + 1) % saves.Length;
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