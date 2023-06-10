using System.Collections.Generic;
using UnityEngine;

public class ItemModifier : ItemMachineComponent, IInfoProvider {
    [Space] [SerializeField] bool _bakeSellPrice = true;
    [SerializeField] Modifier _sellPriceModifier;
    [SerializeField] ItemDataModifier _dataModifier;

    protected override void OnItemEntered(Item item) {
        _dataModifier.Apply(item);
        item.SellPriceModifier += _sellPriceModifier;
        if (_bakeSellPrice) item.SellPriceModifier.Bake();
    }

    public IEnumerable<(string Name, string Value)> GetInfo() {
        yield return ("Multiplier", _sellPriceModifier.ToString());
    }
}