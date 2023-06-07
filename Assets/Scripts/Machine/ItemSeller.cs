using UnityEngine;

public class ItemSeller : ItemMachineComponent {
    [Space]
    [SerializeField] MoneyManager _moneyManager;
    [SerializeField] Modifier _sellPriceModifier;

    protected override void OnItemEntered(Item item) {
        var sellPrice = (item.SellPriceModifier + _sellPriceModifier).Apply(item.BaseSellPrice);
        _moneyManager.CurrentMoney += sellPrice;
    }
}