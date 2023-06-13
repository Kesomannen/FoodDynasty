using UnityEngine;

public class InventoryItemEntity : Entity, IDataProvider<InventoryItemData> {
    [SerializeField] InventoryItemData _data;
    
    public InventoryItemData Data => _data;
    public override string Name => _data.Name;
    public override string Description => _data.Description;
}