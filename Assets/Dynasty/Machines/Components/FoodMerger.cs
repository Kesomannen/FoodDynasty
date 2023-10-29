using System;
using System.Collections.Generic;
using System.Linq;
using Dynasty.Food;
using Dynasty.Library;
using Dynasty.Persistent;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dynasty.Machines {

public class FoodMerger : FoodMachineComponent, IInfoProvider, IAdditionalSaveData<FoodMerger.SaveData> {
    [Space]
    [SerializeField] int _requiredItems = 2;
    [SerializeField] CombineMode _combineMode;
    [SerializeField] Modifier _sellPriceModifier = new(multiplicative: 1f);
    [SerializeField] Supply _supply;
    [SerializeField] Event<FoodBehaviour> _applyEvent;
    [SerializeField] Lookup<CustomObjectPool<Poolable>> _modelLookup;

    Queue<(double Value, CustomObjectPool<Poolable>[] Models)> _mergingItems = new();
    int _currentMergingItems;

    public Supply Supply {
        get => _supply;
        set => _supply = value;
    }
    
    public Event<FoodBehaviour> ApplyEvent {
        get => _applyEvent;
        set => _applyEvent = value;
    }
    
    protected override void OnEnable() {
        base.OnEnable();
        _applyEvent.OnRaised += Apply;
    }
    
    protected override void OnDisable() {
        base.OnDisable();
        _applyEvent.OnRaised -= Apply;
    }

    void Apply(FoodBehaviour food) {
        var values = new double[_requiredItems];
        var pools = new List<CustomObjectPool<Poolable>>();
        for (var i = 0; i < values.Length; i++) {
            var (value, toppings) = _mergingItems.Dequeue();
            values[i] = value;
            pools.AddRange(toppings);
        }
        
        var yieldedPrice = _combineMode switch {
            CombineMode.Add => values.Sum(),
            CombineMode.Average => values.Average(),
            CombineMode.Multiply => values.Aggregate((a, b) => a * b),
            _ => 0
        };
        
        food.SellPrice = new Modifier(@base: yieldedPrice);
        food.ModelProvider.AddToppingModels(pools);
    }
    
    protected override void OnTriggered(FoodBehaviour food) {
        _mergingItems.Enqueue((food.GetSellPrice(), food.ModelProvider.GetModelPools().ToArray()));
        food.Dispose();
        
        _currentMergingItems++;
        if (_currentMergingItems < _requiredItems) return;
        
        _supply.CurrentSupply++;
        _currentMergingItems = 0;
    }

    public IEnumerable<EntityInfo> GetInfo() {
        var combineString = _combineMode switch {
            CombineMode.Add => "(A+B)",
            CombineMode.Average => "Average",
            CombineMode.Multiply => "(AxB)",
            _ => "Unknown"
        };
        yield return new EntityInfo("Multiplier", combineString + _sellPriceModifier);
    }
    
    enum CombineMode {
        Add,
        Average,
        Multiply
    }

    public void OnAfterLoad(SaveData data) {
        _currentMergingItems = data.CurrentMergingItems;
        _mergingItems = new Queue<(double Value, CustomObjectPool<Poolable>[] Models)>(
            data.MergingItems.Select(item => (
                item.Value,
                item.ModelIds.Select(_modelLookup.GetFromId).ToArray()
            )
        ));
    }

    public SaveData GetSaveData() {
        return new SaveData {
            CurrentMergingItems = _currentMergingItems,
            MergingItems = _mergingItems.Select(item => new MergingItem {
                Value = item.Value,
                ModelIds = item.Models.Select(_modelLookup.GetId).ToArray()
            }).ToArray()
        };
    }
    
    [Serializable]
    public struct SaveData {
        public int CurrentMergingItems;
        public MergingItem[] MergingItems;
    }

    [Serializable]
    public struct MergingItem {
        public double Value;
        public int[] ModelIds;
    }
}

}
