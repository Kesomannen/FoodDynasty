using UnityEditor;
using UnityEngine;

public static class ItemCreator {
    static GameObject _baseMachinePrefab;

    static GameObject BaseMachinePrefab {
        get {
            if (_baseMachinePrefab == null) {
                _baseMachinePrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{MachinePrefabPath}/MachineBase.prefab");
            }

            return _baseMachinePrefab;
        }
    }
    
    const string ItemDataPath = "Assets/Data/Items";
    const string FoodPrefabPath = "Assets/Prefabs/Food";
    const string MachinePrefabPath = "Assets/Prefabs/Machines";

    public static MachineItemData CreateGenericMachine(string name, GameObject modelPrefab, ItemTier tier) {
        var prefabSource = (GameObject) PrefabUtility.InstantiatePrefab(BaseMachinePrefab);
        prefabSource.name = name;
        
        PrefabUtility.InstantiatePrefab(modelPrefab, prefabSource.transform);
        var prefab = SavePrefab(prefabSource, GetMachinePath(tier));

        var gridObject = prefab.GetComponent<GridObject>();
        gridObject.BlueprintPrefab = modelPrefab;

        var itemData = ScriptableObject.CreateInstance<MachineItemData>();
        itemData.Image = ThumbnailCreator.Create(modelPrefab, name);
        itemData.name = itemData.Name = name;
        itemData.Prefab = gridObject;

        SaveData(itemData);
        
        return itemData;
    }
    
    static string GetMachinePath(ItemTier tier) => $"{MachinePrefabPath}/{tier}";

    static GameObject SavePrefab(GameObject prefab, string path) {
        return PrefabUtility.SaveAsPrefabAsset(prefab, $"{path}/{prefab.name}.prefab");
    }
    
    static void SaveData(Object itemData) {
        AssetDatabase.CreateAsset(itemData, $"{ItemDataPath}/{itemData.name}.asset");
    }
}