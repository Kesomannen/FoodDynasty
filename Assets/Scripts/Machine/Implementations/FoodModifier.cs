﻿using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class FoodModifier : FoodMachineComponent, IInfoProvider {
    [Space]
    [SerializeField] FoodModifierGroup _modifierGroup;
    [SerializeField] Event<Food> _onItemModified;

    public FoodModifierGroup ModifierGroup {
        get => _modifierGroup;
        set => _modifierGroup = value;
    }
    
    protected override void OnTriggered(Food food) {
        _modifierGroup.Apply(food);

        if (_onItemModified != null) {
            _onItemModified.Raise(food);
        }
    }

    public IEnumerable<(string Name, string Value)> GetInfo() {
        yield return ("Multiplier", _modifierGroup.SellPriceModifier.ToString());
    }
}