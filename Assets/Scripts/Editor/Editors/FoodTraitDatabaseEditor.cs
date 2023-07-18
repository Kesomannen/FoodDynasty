using System.Linq;
using Dynasty.Food.Data;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FoodTraitDatabase))]
public class FoodTraitDatabaseEditor : Editor {
    string _newEntryName;
    FoodTraitType _newEntryType;
    
    public override void OnInspectorGUI() {
        var database = (FoodTraitDatabase) target;
        var entries = database.Entries;

        EditorGUILayout.BeginHorizontal();
        
        _newEntryName = EditorGUILayout.TextField(_newEntryName);
        _newEntryType = (FoodTraitType) EditorGUILayout.EnumPopup(_newEntryType);
        
        EditorGUI.BeginDisabledGroup(
            string.IsNullOrWhiteSpace(_newEntryName) ||
            entries.Any(entry => entry.Name == _newEntryName)
        );
        
        if (GUILayout.Button(EditorGUIUtility.IconContent("CreateAddNew"))) {
            database.AddEntry(_newEntryName, _newEntryType);
            _newEntryName = "";
        } 
        
        EditorGUI.EndDisabledGroup();
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // ReSharper disable once ForCanBeConvertedToForeach
        // Collection was modified exception caused by type change
        var sortedEntries = entries.OrderBy(entry => entry.Name).ToArray();
        for (var i = 0; i < entries.Count; i++) {
            var entry = sortedEntries[i];
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(entry.Name);
            
            var newType = (FoodTraitType) EditorGUILayout.EnumPopup(entry.Type);
            if (newType != entry.Type) {
                database.SetType(entry.Hash, newType);
            }
            
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_TreeEditor.Trash"))) {
                database.RemoveEntry(entry.Hash);
            }

            EditorGUILayout.EndHorizontal();
        } 
    }
}