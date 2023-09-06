using System;
using Dynasty.Persistent.Mapping;
using NaughtyAttributes;
using UnityEngine;

namespace Dynasty.UI.Menu {
    
[CreateAssetMenu(menuName = "UI/Main Menu Panorama")]
public class MainMenuPanorama : ScriptableObject {
    [ReadOnly] [AllowNesting]
    [SerializeField] MachineSaveData _saveData;

    public MachineSaveData SaveData {
        get => _saveData;
        set => _saveData = value;
    }
}

}