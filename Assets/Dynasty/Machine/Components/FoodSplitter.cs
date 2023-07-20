using System;
using System.Collections.Generic;
using Dynasty.Library.Data;
using Dynasty.Food.Instance;
using Dynasty.Library.Entity;
using Dynasty.Library.Classes;
using Dynasty.Library.Events;
using Dynasty.Machine.Internal;
using UnityEngine;

namespace Dynasty.Machine.Components {

public class FoodSplitter : FoodMachineComponent, IInfoProvider, IAdditionalSaveData<FoodSplitter.SaveData> {
    [Space]
    [SerializeField] int _splitsPerItem = 2;
    [SerializeField] Modifier _sellPriceModifier = new(multiplicative: 1f);
    [SerializeField] Supply _supply;
    [SerializeField] Event<FoodBehaviour> _applyEvent;

    Queue<double> _input = new(); 
    int _splitsLeft;

    public Supply Supply {
        get => _supply;
        set => _supply = value;
    }
    
    public Event<FoodBehaviour> ApplyEvent {
        get => _applyEvent;
        set => _applyEvent = value;
    }

    void Awake() {
        _splitsLeft = _splitsPerItem;
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
        if (!_input.TryPeek(out var yieldedPrice)) return;
        food.SellPrice = new Modifier(additive: yieldedPrice);
        
        _splitsLeft--;
        if (_splitsLeft > 0) return;
        
        _splitsLeft = _splitsPerItem;
        _input.TryDequeue(out _);
    }
    
    protected override void OnTriggered(FoodBehaviour food) {
        _supply.CurrentSupply += _splitsPerItem;
        _input.Enqueue(_sellPriceModifier.Apply(food.GetSellPrice()));
        food.Dispose();
    }

    public IEnumerable<EntityInfo> GetInfo() {
        yield return new EntityInfo("Splits", _splitsPerItem.ToString());
        yield return new EntityInfo("Multiplier", _sellPriceModifier.ToString());
    }

    public void OnAfterLoad(SaveData data) {
        _splitsLeft = data.SplitsLeft;
        _input = new Queue<double>(data.Input);
    }

    public SaveData GetSaveData() {
        return new SaveData {
            SplitsLeft = _splitsLeft,
            Input = _input.ToArray()
        };
    }
    
    [Serializable]
    public struct SaveData {
        public int SplitsLeft;
        public double[] Input;

        public override string ToString() {
            return $"SplitsLeft: {SplitsLeft}, " +
                   $"Input: {string.Join(", ", Input)}";
        }
    }
}

}