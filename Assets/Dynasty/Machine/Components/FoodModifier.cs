using System.Collections.Generic;
using Dynasty.Food.Instance;
using Dynasty.Food.Modification;
using Dynasty.Library.Entity;
using Dynasty.Library.Events;
using Dynasty.Machine.Internal;
using UnityEngine;

namespace Dynasty.Machine.Components {

public class FoodModifier : FoodMachineComponent, IInfoProvider {
    [Space]
    [SerializeField] FoodModifierGroup _modifierGroup;
    [SerializeField] Event<FoodBehaviour> _onItemModified;

    public FoodModifierGroup ModifierGroup {
        get => _modifierGroup;
        set => _modifierGroup = value;
    }
    
    public Event<FoodBehaviour> ItemModifiedEvent {
        get => _onItemModified;
        set => _onItemModified = value;
    }
    
    protected override void OnTriggered(FoodBehaviour food) {
        _modifierGroup.Apply(food);

        if (_onItemModified != null) {
            _onItemModified.Raise(food);
        }
    }

    public IEnumerable<EntityInfo> GetInfo() {
        yield return new EntityInfo("Multiplier", _modifierGroup == null ? "N/A" : _modifierGroup.SellPriceModifier.ToString());
    }
}

}