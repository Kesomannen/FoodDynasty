using System.Collections;
using Dynasty.Library.Helpers;
using TMPro;
using UnityEngine;

namespace Dynasty.UI.Displays {

public class FpsDisplay : MonoBehaviour {
    [SerializeField] float _updateInterval = 0.5f;
    [SerializeField] TMP_Text _text;

    void OnEnable() {
        StartCoroutine(UpdateLoop());
    }

    IEnumerator UpdateLoop() {
        while (enabled) {
            _text.text = $"{1f / Time.unscaledDeltaTime:F0} FPS";
            yield return CoroutineHelpers.Wait(_updateInterval);
        }
    }
}

}