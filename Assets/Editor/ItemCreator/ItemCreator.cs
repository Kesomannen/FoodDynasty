using System;
using Dynasty.Core.Grid;
using Dynasty.Core.Inventory;
using Dynasty.Food.Data;
using Dynasty.Food.Modification;
using Dynasty.Food.Instance;
using Dynasty.Library.Entity;
using Dynasty.Library.Events;
using Dynasty.Library.Extensions;
using Dynasty.Machine.Components;
using Dynasty.Machine.Internal;
using Unity.VisualScripting;
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
    
    static MachineFactory _genericFactory;
    static MachineFactory _modifierFactory;
    static MachineFactory _modifierWithSupplyFactory;
    static MachineFactory _sellFactory;
    static MachineFactory _splitFactory;
    static MachineFactory _dispenseFactory;
    
    const string DataPath = "Assets/Data";
    const string FoodPrefabPath = "Assets/Prefabs/Food";
    const string MachinePrefabPath = "Assets/Prefabs/Machines";

    static void CheckInitialized() {
        if (_initialized) return;
        _initialized = true;

        _genericFactory = new MachineFactory();
        
        _modifierFactory = new MachineFactory((item, prefab) => {
            item.SetType(ItemType.Modifier);
            AttachModifier(prefab.gameObject, item.Name);
        });
        
        _modifierWithSupplyFactory = new MachineFactory((item, prefab) => {
            AttachSupplyWithTopping(item, prefab, out var checkEvent, out _);

            foreach (var foodMachineComponent in prefab.GetComponentsInChildren<FoodMachineComponent>()) {
                foodMachineComponent.TriggerEvent.Condition = checkEvent;
            }
        }, _modifierFactory);
        
        _sellFactory = new MachineFactory((item, prefab) => {
            item.SetType(ItemType.Seller);
            prefab.AddComponent<FoodSeller>().TriggerEvent = new FilteredFoodEvent(AttachTrigger(prefab.gameObject));
        });
        
        _splitFactory = new MachineFactory((_, prefab) => {
            var dispenseEvent = prefab.AddComponent<FoodEvent>();
            var condition = prefab.AddComponent<Condition>();
            var foodTriggerEvent = AttachTrigger(prefab.gameObject);
            
            var supply = prefab.AddComponent<Supply>();
            var dispenser = prefab.AddComponent<FoodDispenser>();
            var splitter = prefab.AddComponent<FoodSplitter>();
            
            supply.UseEvent = dispenseEvent;
            supply.Condition = condition;
            
            dispenser.DispenseEvent = dispenseEvent;
            dispenser.Condition = condition;
            
            splitter.TriggerEvent = new FilteredFoodEvent(foodTriggerEvent);
            splitter.ApplyEvent = dispenseEvent;
            splitter.Supply = supply;
        });

        _dispenseFactory = new MachineFactory((item, prefab) => {
            var useEvent = AttachSupplyWithItem<FoodItemData>(item, prefab, out var checkEvent, out _);
            var dispenser = prefab.AddComponent<FoodDispenser>();
            
            dispenser.DispenseEvent = useEvent;
            dispenser.Condition = checkEvent;
        });
    }

    static Event<FoodBehaviour> AttachSupplyWithTopping(MachineItemData machine, Component prefab, out CheckEvent<bool> checkEvent, out ToppingItemData refillItem) {
        var useEvent = AttachSupplyWithItem(machine, prefab, out checkEvent, out refillItem);
        refillItem.AssociatedMachine = machine;
        return useEvent;
    }
    
    static Event<FoodBehaviour> AttachSupplyWithItem<T>(IEntityData entity, Component prefab, out CheckEvent<bool> checkEvent, out T refillItem) where T : ItemData {
        refillItem = ScriptableObject.CreateInstance<T>();
        refillItem.name = refillItem.Name = $"{entity.Name} Refill";
        refillItem.Icon = entity.Icon;

        var useEvent = prefab.AddComponent<Event<FoodBehaviour>>();
        checkEvent = AttachSupply(prefab.gameObject, useEvent, refillItem);

        SaveData(refillItem, "Items");
        return useEvent;
    }

    #region Machine Creation
    
    public static MachineItemData CreateSplitMachine(string name, GameObject modelPrefab, ItemTier tier) {
        CheckInitialized();
        return _splitFactory.Create(name, modelPrefab, tier);
    }

    public static MachineItemData CreateDepositMachine(string name, GameObject modelPrefab, ItemTier tier) {
        CheckInitialized();
        return _dispenseFactory.Create(name, modelPrefab, tier);
    }
    
    public static MachineItemData CreateSellMachine(string name, GameObject modelPrefab, ItemTier tier) {
        CheckInitialized();
        return _sellFactory.Create(name, modelPrefab, tier);
    }
    
    public static MachineItemData CreateModifierMachineWithSupply(string name, GameObject modelPrefab, ItemTier tier) {
        CheckInitialized();
        return _modifierWithSupplyFactory.Create(name, modelPrefab, tier);
    }
    
    public static MachineItemData CreateModifierMachine(string name, GameObject modelPrefab, ItemTier tier) {
        CheckInitialized();
        return _modifierFactory.Create(name, modelPrefab, tier);
    }
    
    public static MachineItemData CreateGenericMachine(string name, GameObject modelPrefab, ItemTier tier) {
        CheckInitialized();
        return _genericFactory.Create(name, modelPrefab, tier);
    }

    #endregion

    static (MachineItemData Item, GridObject Prefab) CopyBaseMachine(string name, GameObject modelPrefab) {
        var prefabSource = (GameObject) PrefabUtility.InstantiatePrefab(BaseMachinePrefab);
        prefabSource.name = name;
        
        PrefabUtility.InstantiatePrefab(modelPrefab, prefabSource.transform);

        var gridObject = prefabSource.GetComponent<GridObject>();
        gridObject.BlueprintPrefab = modelPrefab;

        var itemData = ScriptableObject.CreateInstance<MachineItemData>();
        itemData.Icon = ThumbnailCreator.Create(modelPrefab, name);
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

    static FoodModifier AttachModifier(Event<FoodBehaviour> triggerEvent, string modifierName) {
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

    static Event<FoodBehaviour> AttachTrigger(GameObject prefab) {
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
    
    static Supply AttachSupply(GameObject prefab, CheckEvent<bool> checkEvent, GenericEvent useEvent, ItemData refillItem) {
        var supply = prefab.AddComponent<Supply>();
        
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