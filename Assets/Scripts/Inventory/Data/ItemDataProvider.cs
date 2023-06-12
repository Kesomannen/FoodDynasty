using UnityEngine;

public class ItemDataProvider : MonoBehaviour, IItemDataProvider {
    [SerializeField] InventoryItemData _data;
    
    public InventoryItemData Data => _data;
}