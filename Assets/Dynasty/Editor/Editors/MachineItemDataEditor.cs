using System.Collections.Generic;
using Dynasty.Core.Inventory;
using Dynasty.Library;
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
            
            var providerName = StringHelpers.FormatCamelCase(provider.GetType().Name);
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

        if (GUI.changed) EditorUtility.SetDirty(data);
        
        serializedObject.ApplyModifiedProperties();
    }

    static void DrawImageTools(MachineItemData data) {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Generate Image")) {
            GenerateImage(data);
        }

        if (GUILayout.Button("Generate All Images")) {
            foreach (var item in EditorUtil.FetchAll<MachineItemData>()) {
                GenerateImage(item);
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    static void DrawInfo(IEnumerable<EntityInfo> infos) {
        foreach (var info in infos) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(info.Name, info.Value);
            EditorGUILayout.EndHorizontal();
        }
    }
    
    static void GenerateImage(MachineItemData data) {
        if (data.Prefab == null) return;
        data.Icon = ThumbnailCreator.Create(data.Prefab, data.Name);
        EditorUtility.SetDirty(data);
    }
}