using UnityEngine;

public class ItemSeller : ItemMachineComponent {
    [SerializeField] Modifier _modifier;
    [SerializeField] MoneyManager _moneyManager;

    protected override void OnItemEntered(Item item) {
        var sellPrice = (item.SellPriceModifier + _modifier).Apply(item.BaseSellPrice);
        Debug.Log($"Selling {item.name} for {sellPrice}. Base: {item.BaseSellPrice}");
        _moneyManager.CurrentMoney += sellPrice;
    }
}