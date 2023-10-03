using Dynasty.Persistent.Mapping;
using Dynasty.UI.Menu;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MachineLoader))]
public class MachineLoaderEditor : Editor {
    MainMenuPanorama _panorama;
    
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        
        var machineLoader = (MachineLoader) target;
        _panorama = (MainMenuPanorama) EditorGUILayout.ObjectField(_panorama, typeof(MainMenuPanorama), false);

        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(_panorama == null);
        
        if (GUILayout.Button("Save")) {
            _panorama.SaveData = machineLoader.Get();
            EditorUtility.SetDirty(_panorama);
        }
        
        if (GUILayout.Button("Load")) {
            Clear(machineLoader);
            machineLoader.Load(_panorama.SaveData);
        }
        
        EditorGUI.EndDisabledGroup();
        
        if (GUILayout.Button("Clear")) {
            Clear(machineLoader);
        }
        
        EditorGUILayout.EndHorizontal();
    }

    static void Clear(MachineLoader loader) {
        if (Application.isPlaying) loader.Clear();
        else loader.ClearImmediate();
    }
}