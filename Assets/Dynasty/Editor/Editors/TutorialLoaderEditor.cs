using Dynasty.UI.Tutorial;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TutorialLoader))]
public class TutorialLoaderEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        
        var data = (TutorialLoader) target;
        if (GUILayout.Button("Reset")) {
            data.Reset();
        }
    }
}