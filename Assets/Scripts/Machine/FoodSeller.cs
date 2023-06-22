using System.Collections.Generic;
using UnityEngine;

public class FoodSeller : FoodMachineComponent, IInfoProvider {
    [Space]
    [SerializeField] MoneyManager _moneyManager;
    [SerializeField] Modifier _sellPriceModifier;

    protected override void OnTriggered(Food food) {
        var sellPrice = (food.SellPriceModifier + _sellPriceModifier).Apply(food.BaseSellPrice);
        _moneyManager.CurrentMoney += sellPrice;
        food.Dispose();
    }

    public IEnumerable<(string Name, string Value)> GetInfo() {
        yield return ("Multiplier", _sellPriceModifier.ToString());
    }
}