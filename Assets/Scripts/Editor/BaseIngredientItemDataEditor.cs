using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BaseIngredientItemData))]
public class BaseIngredientItemDataEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        
        var data = (BaseIngredientItemData) target;
        DrawImageTools(data);

        serializedObject.ApplyModifiedProperties();
    }

    static void DrawImageTools(BaseIngredientItemData data) {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Generate Image")) {
            GenerateImage(data);
        }

        if (GUILayout.Button("Generate All Images")) {
            var guids = AssetDatabase.FindAssets("t:BaseIngredientItemData");
            foreach (var guid in guids) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var dataAsset = AssetDatabase.LoadAssetAtPath<BaseIngredientItemData>(path);
                GenerateImage(dataAsset);
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    static void GenerateImage(BaseIngredientItemData data) {
        if (data.Prefab == null) return;
        data.Image = ThumbnailCreator.Create(data.Prefab, data.Name);
        EditorUtility.SetDirty(data);
    }
}