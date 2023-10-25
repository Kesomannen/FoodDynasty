using System;
using System.Collections.Generic;
using Dynasty.Food;
using Dynasty.Library;
using Dynasty.Machines;
using UnityEngine;

namespace Dynasty.Machines {

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
        if (!_input.TryPeek(out var input)) return;
        food.SellPrice = new Modifier(@base: input);
        
        _splitsLeft--;
        if (_splitsLeft > 0) return;
        
        _splitsLeft = _splitsPerItem;
        _input.Dequeue();
    }
    
    protected override void OnTriggered(FoodBehaviour food) {
        _supply.CurrentSupply += _splitsPerItem;
        var inputPrice = _sellPriceModifier.Apply(food.GetSellPrice());
        _input.Enqueue(inputPrice);
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