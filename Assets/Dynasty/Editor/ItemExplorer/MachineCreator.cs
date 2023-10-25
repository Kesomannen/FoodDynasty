using System.Collections.Generic;
using Dynasty.Core.Grid;
using Dynasty.Core.Inventory;
using Dynasty.Food;
using Dynasty.Library;
using Dynasty.Machines;
using UnityEditor;
using UnityEngine;

public class MachineCreator {
    readonly string _name;
    readonly MachineItemData _data;
    readonly GridObject _source;

    readonly List<FoodFilterGroup> _filtersToSave = new();
    readonly List<FoodModifierGroup> _modifiersToSave = new();
    readonly List<ItemData> _itemsToSave = new();
    readonly List<ScriptableObject> _poolsToSave = new();

    static readonly Vector3 _triggerColliderCenter = new(0, 0.4f, 0);
    static readonly Vector3 _triggerColliderSize = new(0.75f, 0.75f, 0.1f);

    public MachineCreator(string name, string description, double price, ItemType type, ItemTier tier, GameObject modelPrefab) {
        _name = name;
        
        var sourceGameObject = (GameObject) PrefabUtility.InstantiatePrefab(ItemCreatorUtil.BaseMachinePrefab);
        sourceGameObject.name = name;
        
        PrefabUtility.InstantiatePrefab(modelPrefab, sourceGameObject.transform);

        _source = sourceGameObject.GetComponent<GridObject>();
        _source.BlueprintPrefab = modelPrefab;

        _data = ScriptableObject.CreateInstance<MachineItemData>();
        _data.Icon = ThumbnailCreator.Create(modelPrefab, name);
        _data.name = _data.Name = name;
        
        _data.Description = description;
        _data.ShortDescription = description;
        _data.SetType(type);
        _data.Tier = tier;
        _data.Price = price;
    }

    public MachineCreator AddSupply(out CheckEvent<bool> condition, out Event<FoodBehaviour> consumeEvent, ItemData refillItem) {
        var supply = AddSupply(out condition, out consumeEvent);
        supply.RefillItem = refillItem;
        return this;
    }
    
    public MachineCreator AddSupply(out CheckEvent<bool> condition, out Event<FoodBehaviour> consumeEvent, string refillName) {
        var supply = AddSupply(out condition, out consumeEvent);
        supply.RefillItemName = refillName;
        return this;
    }

    Supply AddSupply(out CheckEvent<bool> condition, out Event<FoodBehaviour> consumeEvent) {
        var supply = Add<Supply>();
        condition = Add<Condition>();
        consumeEvent = Add<FoodEvent>();
        
        supply.Condition = condition;
        supply.ConsumeEvent = consumeEvent;
        
        return supply;
    }

    public MachineCreator AddModifier(FoodModifierGroup modifierGroup, FilteredFoodEvent trigger, Event<FoodBehaviour> itemModifiedEvent = null) {
        var modifier = Add<FoodModifier>();
        modifier.ModifierGroup = modifierGroup;
        modifier.ItemModifiedEvent = itemModifiedEvent;
        modifier.TriggerEvent = trigger;

        return this;
    }

    public MachineCreator AddSeller(Modifier sellPriceModifier, FilteredFoodEvent trigger) {
        var seller = Add<FoodSeller>();
        seller.SellPriceModifier = sellPriceModifier;
        seller.TriggerEvent = trigger;

        return this;
    }

    public MachineCreator AddTrigger(out Event<FoodBehaviour> triggerEvent, CheckEvent<bool> condition = null, FoodFilterGroup filter = null) {
        triggerEvent = Add<FoodEvent>();
        return AddTrigger(new FilteredFoodEvent(triggerEvent, condition, filter));
    }
    
    public MachineCreator AddTrigger(FilteredFoodEvent triggerEvent) {
        var triggerTransform = new GameObject("Trigger").transform;
        triggerTransform.SetParent(_source.transform);
        triggerTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        
        var triggerCollider = triggerTransform.gameObject.AddComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.center = _triggerColliderCenter;
        triggerCollider.size = _triggerColliderSize;
        
        Add<FoodTrigger>().TriggerEvent = triggerEvent;
        
        return this;
    }

    public MachineCreator GenerateModifierGroup(Modifier modifier, GameObject modelPrefab, ItemModelType modelType, out FoodModifierGroup output) {
        GenerateModifierGroup(modifier, out output);

        var modelSource = (GameObject) PrefabUtility.InstantiatePrefab(modelPrefab);
        var poolable = modelSource.AddComponent<Poolable>();

        var pool = ScriptableObject.CreateInstance<GenericObjectPool>();
        pool.name = $"{_name} Pool";
        pool.Prefab = poolable;
        
        output.ModelModifiers.Add(new FoodModelModifier(pool, modelType));
        ItemCreatorUtil.SavePrefab(poolable, "Food");
        ItemCreatorUtil.SaveData(pool, "Pools");

        _poolsToSave.Add(pool);
        return this;
    }

    public MachineCreator GenerateModifierGroup(Modifier modifier, out FoodModifierGroup output, string name = null) {
        output = ScriptableObject.CreateInstance<FoodModifierGroup>();
        output.SellPriceModifier = modifier;
        output.name = name ?? $"{_name} Modifier";
        
        _modifiersToSave.Add(output);
        return this;
    }
    
    public MachineCreator GenerateFilterGroup(out FoodFilterGroup output, string name = null) {
        output = ScriptableObject.CreateInstance<FoodFilterGroup>();
        output.name = name ?? $"{_name} Filter";
        
        _filtersToSave.Add(output);
        return this;
    }
    
    public MachineCreator GenerateTopping(out ToppingItemData output, string name = null) {
        output = ScriptableObject.CreateInstance<ToppingItemData>();
        output.Name = output.name = name ?? $"{_name} Topping";
        output.AssociatedMachine = _data;
        
        _itemsToSave.Add(output);
        return this;
    }
    
    public MachineCreator SetType(ItemType type) {
        _data.SetType(type);
        return this;
    }
    
    public MachineCreator SetSize(GridSize size) {
        _source.StaticSize = size;
        return this;
    }

    T Add<T>() where T : Component {
        return _source.gameObject.AddComponent<T>();
    }

    public void Save() {
        // Save Prefab
        var savedPrefab = ItemCreatorUtil.SavePrefab(_source, $"Machines/{_data.Tier}");
        savedPrefab.GetOrAddComponent<MachineEntity>().Data = _data;
        Object.DestroyImmediate(_source.gameObject);
        
        // Save Data
        _data.Prefab = savedPrefab.GetComponent<GridObject>();
        ItemCreatorUtil.SaveData(_data, "Items");
        
        // Save Assets
        SaveAssets(_filtersToSave, "Filters");
        SaveAssets(_modifiersToSave, "Modifiers");
        SaveAssets(_itemsToSave, "Items");
        SaveAssets(_poolsToSave, "Pools");

        void SaveAssets<T>(IEnumerable<T> assets, string path) where T : ScriptableObject {
            foreach (var scriptableObject in assets) {
                ItemCreatorUtil.SaveData(scriptableObject, path);
            }
        }
    }
}