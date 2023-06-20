using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class ItemModifier : ItemMachineComponent, IInfoProvider {
    [Expandable]
    [SerializeField] ItemModifierGroup _modifierGroup;
    [SerializeField] Optional<Event<Item>> _onItemModified;

    protected override void OnTriggered(Item item) {
        _modifierGroup.Apply(item);

        if (_onItemModified.Enabled) {
            _onItemModified.Value.Raise(item);
        }
    }

    public IEnumerable<(string Name, string Value)> GetInfo() {
        yield return ("Multiplier", _modifierGroup.SellPriceModifier.ToString());
    }
}