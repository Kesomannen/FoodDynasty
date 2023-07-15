#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Dynasty.UI.Tweening {

[CustomEditor(typeof(TweenProfile))]
public class TweenProfileEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        var profile = (TweenProfile) target;

        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Add Scale Tween")) {
            profile.EnableTweens.Add(new ScaleTweenData());
        }
        
        if (GUILayout.Button("Add Fade Tween")) {
            profile.EnableTweens.Add(new FadeTweenData());
        }
        
        EditorGUILayout.EndHorizontal();
        
        serializedObject.ApplyModifiedProperties();
    }
}

}
#endif