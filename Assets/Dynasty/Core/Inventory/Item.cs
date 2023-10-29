namespace Dynasty {

/// <summary>
/// Represents an item in an inventory.
/// </summary>
public struct Item {
    /// <summary>
    /// The current count of this item, can be 0.
    /// </summary>
    public int Count;
    
    /// <summary>
    /// The data associated with this item.
    /// </summary>
    public ItemData Data;
    
    /// <summary>
    /// The inventory this item currently belongs to.
    /// </summary>
    public InventoryAsset Inventory;
}

}