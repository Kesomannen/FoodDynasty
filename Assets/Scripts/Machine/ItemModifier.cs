using System.Collections.Generic;
using UnityEngine;

public class ItemModifier : ItemMachineComponent, IInfoProvider {
    [Header("Modifier")] 
    [SerializeField] bool _bakeSellPrice = true;
    [SerializeField] Modifier _sellPriceModifier;
    [SerializeField] ItemDataModifier _dataModifier;
    
    [Header("Events")]
    [SerializeField] Optional<Event<Item>> _onItemModified;

    protected override void OnTriggered(Item item) {
        _dataModifier.Apply(item);
        item.SellPriceModifier += _sellPriceModifier;
        
        if (_bakeSellPrice) {
            item.SellPriceModifier.Bake();
        }

        if (_onItemModified.Enabled) {
            _onItemModified.Value.Raise(item);
        }
    }

    public IEnumerable<(string Name, string Value)> GetInfo() {
        yield return ("Multiplier", _sellPriceModifier.ToString());
    }
}