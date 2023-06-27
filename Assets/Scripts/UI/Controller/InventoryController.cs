using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryController : MonoBehaviour {
    [Header("Inventory")] 
    [SerializeField] Transform _itemParent;
    [SerializeField] ContainerObjectPool<Item> _itemPool;
    [SerializeField] GameEvent<Item> _onItemClicked;
    [SerializeField] InventoryAsset _inventoryAsset;
    [Space]
    [SerializeField] bool _descendingOrder;
    [SerializeField] ItemSortingMode _sortingMode;
    [SerializeField] TooltipData<ItemData> _tooltipData;

    readonly Dictionary<ItemData, Container<Item>> _itemContainers = new();
    Container<Item> _selectedContainer;

    void OnEnable() {
        _inventoryAsset.OnItemChanged += OnItemChanged;
        RefreshContainers();
        SortContainers();
    }
    
    void OnDisable() {
        _inventoryAsset.OnItemChanged -= OnItemChanged;
    }

    void OnItemChanged(Item item) {
        if (item.Count <= 0) DestroyContainer(item);
        else CreateOrUpdateContainer(item);
    }

    void CreateOrUpdateContainer(Item item) {
        var created = false;
        if (!_itemContainers.TryGetValue(item.Data, out var itemContainer)) {
            itemContainer = _itemPool.Get(item, _itemParent);

            var interactable = itemContainer.GetOrAddComponent<Interactable>();
            interactable.OnClicked += OnItemClicked;
            interactable.OnHovered += OnItemHovered;
            
            _itemContainers.Add(item.Data, itemContainer);
            created = true;
        }
        
        itemContainer.SetContent(item);
        
        if (created) SortContainers();
    }

    void DestroyContainer(Item item) {
        if (!_itemContainers.TryGetValue(item.Data, out var itemContainer)) return;
        
        var interactable = itemContainer.GetOrAddComponent<Interactable>();
        interactable.OnClicked -= OnItemClicked;
        interactable.OnHovered -= OnItemHovered;

        if (_selectedContainer == itemContainer) {
            _tooltipData.Hide();
        }   
        
        _itemContainers.Remove(item.Data);
        itemContainer.Dispose();
    }
    
    void RefreshContainers() {
        foreach (var item in _inventoryAsset.Items) {
            OnItemChanged(item);
        }
    }

    void SortContainers() {
        _itemContainers.Values.SortSiblingIndices(
            container => container.Content.Data, 
            ItemSortingUtil.GetComparison(_sortingMode, _descendingOrder)
        );
    }

    void OnItemClicked(Interactable interactable, PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        _onItemClicked.Raise(GetContainer(interactable).Content);
    }
    
    void OnItemHovered(Interactable interactable, bool hovered, PointerEventData eventData) {
        var container = GetContainer(interactable);
        _tooltipData.Show(container.Content.Data, hovered);
        _selectedContainer = hovered ? container : null;
    }

    Container<Item> GetContainer(Component obj) {
        return _itemContainers.Values.First(container => container.gameObject == obj.gameObject);
    }
}