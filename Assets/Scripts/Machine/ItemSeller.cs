using System.Collections.Generic;
using UnityEngine;

public class ItemSeller : ItemMachineComponent, IInfoProvider {
    [Space]
    [SerializeField] MoneyManager _moneyManager;
    [SerializeField] Modifier _sellPriceModifier;

    protected override void OnTriggered(Item item) {
        var sellPrice = (item.SellPriceModifier + _sellPriceModifier).Apply(item.BaseSellPrice);
        _moneyManager.CurrentMoney += sellPrice;
        item.Dispose();
    }

    public IEnumerable<(string Name, string Value)> GetInfo() {
        yield return ("Multiplier", _sellPriceModifier.ToString());
    }
}