using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FoodItemData))]
public class BaseIngredientItemDataEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        
        var data = (FoodItemData) target;
        DrawImageTools(data);

        serializedObject.ApplyModifiedProperties();
    }

    static void DrawImageTools(FoodItemData data) {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Generate Image")) {
            GenerateImage(data);
        }

        if (GUILayout.Button("Generate All Images")) {
            var guids = AssetDatabase.FindAssets("t:BaseIngredientItemData");
            foreach (var guid in guids) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var dataAsset = AssetDatabase.LoadAssetAtPath<FoodItemData>(path);
                GenerateImage(dataAsset);
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    static void GenerateImage(FoodItemData data) {
        if (data.Prefab == null) return;
        data.Icon = ThumbnailCreator.Create(data.Prefab, data.Name);
        EditorUtility.SetDirty(data);
    }
}