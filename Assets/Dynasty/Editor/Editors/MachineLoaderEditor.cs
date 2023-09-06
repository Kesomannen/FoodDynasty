using Dynasty.Persistent.Mapping;
using Dynasty.UI.Menu;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MachineLoader))]
public class MachineLoaderEditor : Editor {
    MainMenuPanorama _panorama;
    
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        EditorGUILayout.BeginHorizontal();
        
        _panorama = (MainMenuPanorama) EditorGUILayout.ObjectField(_panorama, typeof(MainMenuPanorama), false);
        if (GUILayout.Button("Save")) {
            var machineLoader = (MachineLoader) target;
            _panorama.SaveData = machineLoader.Get();
        }
        
        EditorGUILayout.EndHorizontal();
    }
}