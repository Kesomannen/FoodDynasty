using Dynasty.Library;
using NaughtyAttributes;
using UnityEngine;

namespace Dynasty.UI.Tutorial {

public class TutorialLoader : MonoBehaviour {
    [SerializeField] GameEvent<int> _loadSceneEvent;
    [SerializeField] int _gameSceneId;
    
    public void Reset() {
        SettingsFactory.Get("seen_tutorial", 0).Value = 0;
    }
    
    void Start() {
        var setting = SettingsFactory.Get("seen_tutorial", 0);
        if (setting.Value != 0) return;
        
        setting.Value = 1;
        TutorialManager.Trigger();
        _loadSceneEvent.Raise(_gameSceneId);
    }
}

}