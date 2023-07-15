using System.Collections.Generic;
using Dynasty.Food.Instance;
using Dynasty.Library.Entity;
using Dynasty.Library.Classes;
using Dynasty.Library.Events;
using Dynasty.Machine.Internal;
using UnityEngine;

namespace Dynasty.Machine.Components {

public class YieldInputFood : FoodMachineComponent, IInfoProvider {
    [Space]
    [SerializeField] int _yieldPerItem;
    [SerializeField] Modifier _sellPriceModifier = new(multiplicative: 1f);
    [SerializeField] Supply _supply;
    [SerializeField] Event<FoodBehaviour> _applyEvent;

    readonly Queue<double> _yieldedPrices = new();
    int _yieldsLeft;

    void Awake() {
        _yieldsLeft = _yieldPerItem;
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
        if (!_yieldedPrices.TryPeek(out var yieldedPrice)) return;
        food.SellPrice = new Modifier(additive: yieldedPrice);
        
        _yieldsLeft--;
        if (_yieldsLeft > 0) return;
        
        _yieldsLeft = _yieldPerItem;
        _yieldedPrices.Dequeue();
    }
    
    protected override void OnTriggered(FoodBehaviour food) {
        _supply.CurrentSupply += _yieldPerItem;
        _yieldedPrices.Enqueue(_sellPriceModifier.Apply(food.GetSellPrice()));
    }


    public IEnumerable<EntityInfo> GetInfo() {
        yield return new EntityInfo("Yield", _yieldPerItem.ToString());
        yield return new EntityInfo("Multiplier", _sellPriceModifier.ToString());
    }
}

}