using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UITweenProfile))]
public class UITweenProfileEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        var profile = (UITweenProfile) target;

        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Add Scale Twee")) {
            profile.EnableTweens.Add(new ScaleTweenData());
        }
        
        if (GUILayout.Button("Add Fade Tween")) {
            profile.EnableTweens.Add(new FadeTweenData());
        }
        
        EditorGUILayout.EndHorizontal();
        
        serializedObject.ApplyModifiedProperties();
    }
}