using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SaveManager))]
public class SaveManagerEditor : Editor {
    public override async void OnInspectorGUI() {
        base.OnInspectorGUI();
        
        var saveManager = (SaveManager) target;
        
        EditorGUI.BeginDisabledGroup(true);
        
        var state = saveManager.State;
        if (state != null) {
            foreach (var key in state.Keys) {
                var valueString = state[key].ToString();
                var height = GetTextAreaHeight(valueString);
                
                EditorGUILayout.LabelField(key, valueString, GUILayout.Height(height));
            }   
        }
        
        EditorGUI.EndDisabledGroup();
        
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Save to loader")) {
            await saveManager.SaveCurrent();
        }

        if (GUILayout.Button("Load from loader")) {
            await saveManager.LoadCurrent();
        }
        
        if (GUILayout.Button("Delete current slot")) {
            saveManager.DeleteSlot(saveManager.CurrentSaveSlot);
        }
        
        EditorGUILayout.EndHorizontal();
    }

    static int GetNumberOfLines(string text) {
        var content = Regex.Replace(text, @"\r\n|\n\r|\r|\n", Environment.NewLine);
        var lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        return lines.Length;
    }

    static float GetTextAreaHeight(string text) {
        return (EditorGUIUtility.singleLineHeight - 3.0f) * GetNumberOfLines(text) + 3.0f;
    }
}