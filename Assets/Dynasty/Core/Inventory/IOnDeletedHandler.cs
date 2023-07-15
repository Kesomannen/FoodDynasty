namespace Dynasty.Core.Inventory {

public interface IOnDeletedHandler {
    void OnDeleted(InventoryAsset toInventory);
}

}