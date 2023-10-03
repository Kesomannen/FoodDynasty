using System.Collections;
using Dynasty.Library.Extensions;
using Dynasty.Library.Helpers;
using Dynasty.Machines;
using Dynasty.Persistent.Mapping;
using UnityEngine;
using UnityEngine.UI;

namespace Dynasty.UI.Menu {

public class MainMenuBackground : MonoBehaviour {
    [SerializeField] float _duration;
    [SerializeField] MachineLoader _loader;
    [SerializeField] MainMenuPanorama[] _panoramas;
    [Space]
    [SerializeField] float _fadeDuration;
    [SerializeField] float _blackDuration;
    [SerializeField] RectTransform _overlay;

    IEnumerator Start() {
        while (enabled) {
            _loader.Clear();
            _loader.Load(_panoramas.GetRandom().SaveData);
            
            foreach (var supply in FindObjectsOfType<Supply>()) {
                supply.CurrentSupply = Supply.MaxSupply;
            }
            
            LeanTween.alpha(_overlay, 0, _fadeDuration);
            yield return CoroutineHelpers.Wait(_duration + _fadeDuration);
            LeanTween.alpha(_overlay, 1, _fadeDuration);
            yield return CoroutineHelpers.Wait(_fadeDuration + _blackDuration);
        }
    }
}

}