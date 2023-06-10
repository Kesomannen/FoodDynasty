using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MachineItemData))]
public class MachineItemDataEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        
        var data = (MachineItemData) target;

        if (data.Prefab != null) {
            DrawImageTools(data);
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        
        EditorGUILayout.LabelField("Info", EditorStyles.boldLabel);
        if (GUILayout.Button("Refresh")) {
            data.RefreshProviders();
        }
        
        EditorGUILayout.EndHorizontal();

        DrawInfo(data.GetInfo());
        
        if (data.Prefab == null) return;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Providers", EditorStyles.boldLabel);
        
        var providers = data.GetInfoProviders();
        for (var i = 0; i < providers.Length; i++) {
            var (provider, enabled) = providers[i];
            EditorGUILayout.BeginHorizontal();
            
            var providerName = StringUtil.FormatCamelCase(provider.GetType().Name);
            enabled = EditorGUILayout.Toggle(providerName, enabled);
            
            EditorGUI.indentLevel++;
            EditorGUI.BeginDisabledGroup(!enabled);
            EditorGUILayout.EndHorizontal();
            
            DrawInfo(provider.GetInfo());

            EditorGUI.indentLevel--;
            EditorGUI.EndDisabledGroup();
            providers[i] = (provider, enabled);
        }
        data.SetInfoProviders(providers);

        serializedObject.ApplyModifiedProperties();
    }

    static void DrawImageTools(MachineItemData data) {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Generate Image")) {
            GenerateImage(data);
        }

        if (GUILayout.Button("Generate All Images")) {
            var guids = AssetDatabase.FindAssets("t:MachineItemData");
            foreach (var guid in guids) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var dataAsset = AssetDatabase.LoadAssetAtPath<MachineItemData>(path);
                GenerateImage(dataAsset);
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    static void DrawInfo(IEnumerable<(string Name, string Value)> info) {
        foreach (var (infoName, value) in info) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(infoName, value);
            EditorGUILayout.EndHorizontal();
        }
    }
    
    static void GenerateImage(MachineItemData data) {
        if (data.Prefab == null) return;
        data.Image = ThumbnailCreator.Create(data.Prefab, data.Name);
        EditorUtility.SetDirty(data);
    }
}