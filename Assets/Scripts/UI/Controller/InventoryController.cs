using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryController : MonoBehaviour {
    [Header("Inventory")] 
    [SerializeField] Transform _itemParent;
    [SerializeField] InteractableContainer<InventoryItem> _itemPrefab;
    [SerializeField] GameEvent<InventoryItem> _onItemClicked;
    [SerializeField] InventoryAsset _inventoryAsset;
    [Space]
    [SerializeField] TooltipData<InventoryItemData> _tooltipData;

    readonly Dictionary<InventoryItemData, InteractableContainer<InventoryItem>> _itemContainers = new();
    
    void OnEnable() {
        _inventoryAsset.OnItemChanged += OnItemChanged;
        RefreshContainers();
    }
    
    void OnDisable() {
        _inventoryAsset.OnItemChanged -= OnItemChanged;
    }

    void OnItemChanged(InventoryItem item) {
        if (item.Count <= 0) RemoveContainer(item);
        else CreateOrUpdateContainer(item);
    }

    void CreateOrUpdateContainer(InventoryItem item) {
        if (!_itemContainers.TryGetValue(item.Data, out var itemContainer)) {
            itemContainer = Instantiate(_itemPrefab, _itemParent);
            itemContainer.OnClicked += OnItemClicked;
            itemContainer.OnHovered += OnItemHovered;
            
            _itemContainers.Add(item.Data, itemContainer);
        }
        itemContainer.SetContent(item);
    }
    
    void RemoveContainer(InventoryItem item) {
        if (!_itemContainers.TryGetValue(item.Data, out var itemContainer)) return;
        
        itemContainer.OnClicked -= OnItemClicked;
        itemContainer.OnHovered -= OnItemHovered;
        
        _itemContainers.Remove(item.Data);
        Destroy(itemContainer.gameObject);
    }
    
    void RefreshContainers() {
        foreach (var item in _inventoryAsset.Items) {
            OnItemChanged(item);
        }
    }

    void OnItemClicked(InteractableContainer<InventoryItem> itemContainer, PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        _onItemClicked.Raise(itemContainer.Content);
    }
    
    void OnItemHovered(InteractableContainer<InventoryItem> itemContainer, bool hovered, PointerEventData eventData) {
        _tooltipData.Show(itemContainer.Content.Data, hovered);
    }
}