using UnityEngine;

public class ItemModifier : ItemMachineComponent {
    [SerializeField] Modifier _sellPriceModifier;

    protected override void OnItemEntered(Item item) {
        item.SellPriceModifier += _sellPriceModifier;
    }
}