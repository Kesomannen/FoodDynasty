using UnityEngine;

public class InventoryItemDataComponent : UIComponent<Item> {
    [SerializeField] Container<ItemData> _dataContainer;

    public override void SetContent(Item content) {
        _dataContainer.SetContent(content.Data);
    }
}