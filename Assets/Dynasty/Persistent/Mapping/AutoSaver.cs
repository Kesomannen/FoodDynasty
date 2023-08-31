using Dynasty.Library.Helpers;
using Dynasty.Persistent.Core;
using UnityEngine;

namespace Dynasty.Persistent.Mapping {

public class AutoSaver : MonoBehaviour {
    [SerializeField] float _saveInterval = 60;
    [SerializeField] SaveManager _saveManager;

    async void Start() {
        while (enabled) {
            await TaskHelpers.Delay(_saveInterval);
            await _saveManager.Save();
        }
    }
}

}