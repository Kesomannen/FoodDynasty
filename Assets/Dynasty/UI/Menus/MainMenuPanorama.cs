using System;
using Dynasty.Persistent;
using NaughtyAttributes;
using UnityEngine;

namespace Dynasty.UI {
    
[CreateAssetMenu(menuName = "UI/Main Menu Panorama")]
public class MainMenuPanorama : ScriptableObject {
    [SerializeField] Vector2Int _size;
    [ReadOnly] [AllowNesting]
    [SerializeField] MachineSaveData _saveData;

    public MachineSaveData SaveData {
        get => _saveData;
        set => _saveData = value;
    }
    
    public Vector2Int Size => _size;
}

}