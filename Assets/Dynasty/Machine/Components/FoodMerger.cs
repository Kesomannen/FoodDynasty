using System;
using System.Collections.Generic;
using System.Linq;
using Dynasty.Library.Data;
using Dynasty.Food.Instance;
using Dynasty.Library.Entity;
using Dynasty.Library.Classes;
using Dynasty.Library.Events;
using Dynasty.Machine.Internal;
using UnityEngine;

namespace Dynasty.Machine.Components {

public class FoodMerger : FoodMachineComponent, IInfoProvider, IAdditionalSaveData<FoodMerger.SaveData> {
    [Space]
    [SerializeField] int _requiredItems = 2;
    [SerializeField] CombineMode _combineMode;
    [SerializeField] Modifier _sellPriceModifier = new(multiplicative: 1f);
    [SerializeField] Supply _supply;
    [SerializeField] Event<FoodBehaviour> _applyEvent;

    Queue<double> _mergingItemValues = new();
    int _currentMergingItems;
    int _mergedItems;

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
        if (_mergedItems == 0) return;
        _mergedItems--;

        var values = new double[_requiredItems];
        for (var i = 0; i < values.Length; i++) {
            values[i] = _mergingItemValues.Dequeue();
        }
        
        var yieldedPrice = _combineMode switch {
            CombineMode.Add => values.Sum(),
            CombineMode.Average => values.Average(),
            CombineMode.Multiply => values.Aggregate((a, b) => a * b),
            _ => 0
        };
        food.SellPrice = new Modifier(additive: yieldedPrice);
    }
    
    protected override void OnTriggered(FoodBehaviour food) {
        _mergingItemValues.Enqueue(_sellPriceModifier.Apply(food.GetSellPrice()));
        food.Dispose();
        
        _currentMergingItems++;
        if (_currentMergingItems < _requiredItems) return;
        
        _supply.CurrentSupply++;
        _currentMergingItems = 0;
    }

    public IEnumerable<EntityInfo> GetInfo() {
        yield return new EntityInfo("Required Items", _requiredItems.ToString());
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
        _mergedItems = data.MergedItems;
        _currentMergingItems = data.CurrentMergingItems;
        _mergingItemValues = new Queue<double>(data.MergingItemValues);
    }

    public SaveData GetSaveData() {
        return new SaveData {
            MergedItems = _mergedItems,
            CurrentMergingItems = _currentMergingItems,
            MergingItemValues = _mergingItemValues.ToArray()
        };
    }
    
    [Serializable]
    public struct SaveData {
        public int MergedItems;
        public int CurrentMergingItems;
        public double[] MergingItemValues;

        public override string ToString() {
            return $"MergedItems: {MergedItems}, " +
                   $"CurrentMergingItems: {CurrentMergingItems}," +
                   $"MergingItemValues: {MergingItemValues}";
        }
    }
}

}
