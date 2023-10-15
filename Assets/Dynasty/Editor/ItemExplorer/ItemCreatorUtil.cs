using UnityEditor;
using UnityEngine;

public static class ItemCreatorUtil {
    static GameObject _baseMachinePrefab;

    public static GameObject BaseMachinePrefab {
        get {
            if (_baseMachinePrefab == null) {
                _baseMachinePrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabPath}/Machines/MachineBase.prefab");
            }
            return _baseMachinePrefab;
        }
    }
    
    const string DataPath = "Assets/Data";
    const string PrefabPath = "Assets/_Prefabs";
    
    public static void SaveData<T>(T data, string path) where T : ScriptableObject {
        AssetDatabase.CreateAsset(data, $"{DataPath}/{path}/{data.name}.asset");
        AssetDatabase.SaveAssets();
    }
    
    public static GameObject SavePrefab(Component prefab, string path) {
        var gameObject = prefab.gameObject;
        return PrefabUtility.SaveAsPrefabAsset(gameObject, $"{PrefabPath}/{path}/{gameObject.name}.prefab");
    }
}