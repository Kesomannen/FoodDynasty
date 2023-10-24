using System.Collections;
using System.Linq;
using Dynasty.Library.Extensions;
using Dynasty.Library.Helpers;
using Dynasty.Machines;
using Dynasty.Persistent;
using Dynasty.Persistent.Mapping;
using UnityEngine;
using UnityEngine.UI;

namespace Dynasty.UI.Menu {

public class MainMenuBackground : MonoBehaviour {
    [SerializeField] float _duration;
    [SerializeField] MachineLoader _loader;
    [SerializeField] SaveManager _saveManager;
    [SerializeField] MainMenuPanorama _fallbackPanorama;
    [Space]
    [SerializeField] float _fadeDuration;
    [SerializeField] float _blackDuration;
    [SerializeField] RectTransform _overlay;

    async void Start() {
        var i = 0;
        var slots = (await _saveManager.SaveLoader.GetSaves()).ToArray();
        
        while (enabled) {
            MachineSaveData saveData;
            if (slots.Length == 0) {
                saveData = _fallbackPanorama.SaveData;
            } else {
                var state = await _saveManager.SaveLoader.Load(slots[i].Id);

                if (state.TryGetValue("machines", out var saveDataObj))
                    saveData = (MachineSaveData) saveDataObj;
                else
                    saveData = _fallbackPanorama.SaveData;

                i = (i + 1) % slots.Length;
            }
            
            _loader.Clear();
            _loader.Load(saveData);
            
            foreach (var supply in FindObjectsOfType<Supply>()) {
                supply.CurrentSupply = 10000;
            }
            
            LeanTween.alpha(_overlay, 0, _fadeDuration);
            await TaskHelpers.Wait(_duration + _fadeDuration);
            LeanTween.alpha(_overlay, 1, _fadeDuration);
            await TaskHelpers.Wait(_fadeDuration + _blackDuration);
        }
    }
}

}