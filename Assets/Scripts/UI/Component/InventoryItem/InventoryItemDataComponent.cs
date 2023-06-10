using UnityEngine;

public class InventoryItemDataComponent : UIComponent<InventoryItem> {
    [SerializeField] Container<InventoryItemData> _dataContainer;

    public override void SetContent(InventoryItem content) {
        _dataContainer.SetContent(content.Data);
    }
}