using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public static class ItemCreator {
    static bool _initialized;
    static GameObject _baseMachinePrefab;

    static readonly Vector3 _triggerColliderCenter = new(0, 0.4f, 0);
    static readonly Vector3 _triggerColliderSize = new(0.75f, 0.75f, 0.1f);

    static GameObject BaseMachinePrefab {
        get {
            if (_baseMachinePrefab == null) {
                _baseMachinePrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{MachinePrefabPath}/MachineBase.prefab");
            }

            return _baseMachinePrefab;
        }
    }

    static MachineFactory _genericMachineFactory;
    static MachineFactory _modifierMachineFactory;
    static MachineFactory _modifierMachineSupplyFactory;
    
    const string DataPath = "Assets/Data";
    const string FoodPrefabPath = "Assets/Prefabs/Food";
    const string MachinePrefabPath = "Assets/Prefabs/Machines";

    static void CheckInitialized() {
        if (_initialized) return;
        _initialized = true;

        _genericMachineFactory = new MachineFactory();
        
        _modifierMachineFactory = new MachineFactory((item, prefab) => {
            item.SetType(ItemType.Modifier);
            AttachModifier(prefab.gameObject, item.Name);
        });
        
        _modifierMachineSupplyFactory = new MachineFactory((item, prefab) => {
            
            var refillItem = ScriptableObject.CreateInstance<ToppingItemData>();
            refillItem.name = refillItem.Name = $"{item.Name} Refill";
            refillItem.AssociatedMachine = item;
            refillItem.Image = item.Image;
            SaveData(refillItem, "Items");

            var useEvent = prefab.GetComponent<Event<Food>>();
            var checkEvent = AttachSupply(prefab.gameObject, useEvent, refillItem);
            foreach (var foodMachineComponent in prefab.GetComponentsInChildren<FoodMachineComponent>()) {
                foodMachineComponent.TriggerEvent.Condition = checkEvent;
            }
            
        }, _modifierMachineFactory);
    }

    #region Machine Creation

    public static MachineItemData CreateModifierMachineWithSupply(string name, GameObject modelPrefab, ItemTier tier) {
        CheckInitialized();
        return _modifierMachineSupplyFactory.Create(name, modelPrefab, tier);
    }
    
    public static MachineItemData CreateModifierMachine(string name, GameObject modelPrefab, ItemTier tier) {
        CheckInitialized();
        return _modifierMachineFactory.Create(name, modelPrefab, tier);
    }
    
    public static MachineItemData CreateGenericMachine(string name, GameObject modelPrefab, ItemTier tier) {
        CheckInitialized();
        return _genericMachineFactory.Create(name, modelPrefab, tier);
    }

    #endregion

    static (MachineItemData Item, GridObject Prefab) CopyBaseMachine(string name, GameObject modelPrefab) {
        var prefabSource = (GameObject) PrefabUtility.InstantiatePrefab(BaseMachinePrefab);
        prefabSource.name = name;
        
        PrefabUtility.InstantiatePrefab(modelPrefab, prefabSource.transform);

        var gridObject = prefabSource.GetComponent<GridObject>();
        gridObject.BlueprintPrefab = modelPrefab;

        var itemData = ScriptableObject.CreateInstance<MachineItemData>();
        itemData.Image = ThumbnailCreator.Create(modelPrefab, name);
        itemData.name = itemData.Name = name;

        prefabSource.GetOrAddComponent<MachineEntity>().Data = itemData;

        return (itemData, gridObject);
    }

    static void SaveMachine(MachineItemData item, GridObject prefabSource, ItemTier tier) {
        var savedPrefab = SavePrefab(prefabSource, GetMachinePath(tier));
        Object.DestroyImmediate(prefabSource);
        
        item.Prefab = savedPrefab.GetComponent<GridObject>();
        item.Tier = tier;
        
        SaveData(item, "Items");
    }

    static FoodModifier AttachModifier(Event<Food> triggerEvent, string modifierName) {
        var modifier = triggerEvent.gameObject.AddComponent<FoodModifier>();
        modifier.TriggerEvent = new FilteredFoodEvent(triggerEvent);
        
        var modifierGroup = ScriptableObject.CreateInstance<FoodModifierGroup>();
        modifierGroup.name = $"{modifierName} Modifier";
        modifier.ModifierGroup = modifierGroup;
        SaveData(modifierGroup, "Modifiers");
        
        return modifier;
    }
    
    static FoodModifier AttachModifier(GameObject prefab, string modifierName) {
        return AttachModifier(AttachTrigger(prefab), modifierName);
    }

    static Event<Food> AttachTrigger(GameObject prefab) {
        var trigger = new GameObject("Trigger");
        trigger.transform.SetParent(prefab.transform);
        trigger.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        
        var triggerCollider = trigger.AddComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.center = _triggerColliderCenter;
        triggerCollider.size = _triggerColliderSize;
        
        var foodTriggerComponent = prefab.AddComponent<FoodTrigger>();
        var triggerEvent = prefab.AddComponent<FoodEvent>();
        foodTriggerComponent.TriggerEvent = new FilteredFoodEvent(triggerEvent);
        return triggerEvent;
    }

    static CheckEvent<bool> AttachSupply(GameObject prefab, GenericEvent useEvent, ItemData refillItem) {
        return AttachSupply(prefab, prefab.AddComponent<Condition>(), useEvent, refillItem).Condition;
    }
    
    static MachineSupply AttachSupply(GameObject prefab, CheckEvent<bool> checkEvent, GenericEvent useEvent, ItemData refillItem) {
        var supply = prefab.AddComponent<MachineSupply>();
        
        supply.RefillItem = refillItem;
        supply.Condition = checkEvent;
        supply.UseEvent = useEvent;

        return supply;
    }

    static string GetMachinePath(ItemTier tier) => $"{MachinePrefabPath}/{tier}";

    static GameObject SavePrefab(Component prefab, string path) {
        return PrefabUtility.SaveAsPrefabAsset(prefab.gameObject, $"{path}/{prefab.name}.prefab");
    }

    static void SaveData(Object data, string type) {
        AssetDatabase.CreateAsset(data, $"{DataPath}/{type}/{data.name}.asset");
    }

    class MachineFactory {
        readonly Action<MachineItemData, GridObject> _modifyAction;
        readonly MachineFactory[] _subFactories;
        
        public MachineFactory(Action<MachineItemData, GridObject> modifyAction = null, params MachineFactory[] subFactories) {
            _modifyAction = modifyAction;
            _subFactories = subFactories;
        }
        
        public MachineItemData Create(string name, GameObject modelPrefab, ItemTier tier) {
            var (itemData, gridObject) = CopyBaseMachine(name, modelPrefab);
            Modify(itemData, gridObject);
            SaveMachine(itemData, gridObject, tier);
            return itemData;
        }
        
        void Modify(MachineItemData itemData, GridObject gridObject) {
            foreach (var subFactory in _subFactories) {
                subFactory?.Modify(itemData, gridObject);
            }
            _modifyAction?.Invoke(itemData, gridObject);
        }
    }
}