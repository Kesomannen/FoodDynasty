using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class FoodModifier : FoodMachineComponent, IInfoProvider {
    [SerializeField] FoodModifierGroup _modifierGroup;
    [SerializeField] Optional<Event<Food>> _onItemModified;

    protected override void OnTriggered(Food food) {
        _modifierGroup.Apply(food);

        if (_onItemModified.Enabled) {
            _onItemModified.Value.Raise(food);
        }
    }

    public IEnumerable<(string Name, string Value)> GetInfo() {
        yield return ("Multiplier", _modifierGroup.SellPriceModifier.ToString());
    }
}