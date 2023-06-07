using UnityEngine;

public class ItemModifier : ItemMachineComponent {
    [Space] 
    [SerializeField] bool _bakeSellPrice;
    [SerializeField] Modifier _sellPriceModifier;
    [SerializeField] ItemDataModifier _dataModifier;

    protected override void OnItemEntered(Item item) {
        _dataModifier.Apply(item);
        item.SellPriceModifier += _sellPriceModifier;
        if (_bakeSellPrice) item.SellPriceModifier.Bake();
    }
}