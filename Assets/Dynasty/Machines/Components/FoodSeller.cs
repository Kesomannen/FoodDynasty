using System.Collections.Generic;
using Dynasty.Core.Inventory;
using Dynasty.Food.Instance;
using Dynasty.Library;
using Dynasty.Library.Classes;
using Dynasty.Machines;
using UnityEngine;

namespace Dynasty.Machines {

public class FoodSeller : FoodMachineComponent, IInfoProvider {
    [Space]
    [SerializeField] MoneyManager _moneyManager;
    [SerializeField] Modifier _sellPriceModifier;
    
    public Modifier SellPriceModifier {
        get => _sellPriceModifier;
        set => _sellPriceModifier = value;
    }

    protected override void OnTriggered(FoodBehaviour food) {
        if (!food.IsSellable) return;
        
        var sellPrice = (food.SellPrice + _sellPriceModifier).Delta;
        _moneyManager.CurrentMoney += sellPrice;
        food.Dispose();
    }

    public IEnumerable<EntityInfo> GetInfo() {
        yield return new EntityInfo("Multiplier", _sellPriceModifier.ToString());
    }
}

}