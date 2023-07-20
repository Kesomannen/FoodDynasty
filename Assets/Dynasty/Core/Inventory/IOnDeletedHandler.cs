namespace Dynasty.Core.Inventory {

/// <summary>
/// Implement this to receive a callback when this machine is deleted.
/// </summary>
public interface IOnDeletedHandler {
    /// <summary>
    /// Called when this machine is deleted.
    /// </summary>
    /// <param name="toInventory">The inventory this machine was returned to.</param>
    void OnDeleted(InventoryAsset toInventory);
}

}