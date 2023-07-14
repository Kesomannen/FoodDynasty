using System.Collections.Generic;
using Dynasty.Library.Classes;
using UnityEngine;

public class FoodSeller : FoodMachineComponent, IInfoProvider {
    [Space]
    [SerializeField] MoneyManager _moneyManager;
    [SerializeField] Modifier _sellPriceModifier;

    protected override void OnTriggered(Food food) {
        if (!food.IsSellable) return;
        
        var sellPrice = (food.SellPrice + _sellPriceModifier).Delta;
        _moneyManager.CurrentMoney += sellPrice;
        food.Dispose();
    }

    public IEnumerable<EntityInfo> GetInfo() {
        yield return new EntityInfo("Multiplier", _sellPriceModifier.ToString());
    }
}