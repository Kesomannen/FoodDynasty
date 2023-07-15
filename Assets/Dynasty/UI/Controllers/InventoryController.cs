using System;
using System.Collections.Generic;
using System.Linq;
using Dynasty.Core.Inventory;
using Dynasty.Core.Tooltip;
using Dynasty.Library.Classes;
using Dynasty.Library.Events;
using Dynasty.Library.Extensions;
using Dynasty.Library.Helpers;
using Dynasty.UI.Components;
using Dynasty.UI.Controllers;
using Dynasty.UI.Controls;
using Dynasty.UI.Miscellaneous;
using Dynasty.UI.Tooltip;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dynasty.UI.Controllers {

public class InventoryController : MonoBehaviour {
    [Header("References")] 
    [SerializeField] InventoryAsset _inventoryAsset;
    [SerializeField] MoneyManager _moneyManager;

    [Header("Items")]
    [SerializeField] Transform _itemParent;
    [SerializeField] ContainerObjectPool<Item> _itemPool;
    [SerializeField] GameEvent<Item> _onItemClicked;
    [Space]
    [SerializeField] bool _descendingOrder;
    [SerializeField] ItemSortingMode _sortingMode;
    
    [Header("Tooltip")]
    [SerializeField] ItemSellControl _sellControl;
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
            SortingUtil.GetComparison(_sortingMode, _descendingOrder)
        );
    }

    void OnItemClicked(Interactable interactable, PointerEventData eventData) {
        var item = GetContainer(interactable).Content;
        
        switch (eventData.button) {
            case PointerEventData.InputButton.Left:
                _onItemClicked.Raise(item); break;
            case PointerEventData.InputButton.Right:
                _sellControl.Initialize(item, count => TrySell(item.Data, count));
                _sellControl.gameObject.SetActive(true);
                break;
            case PointerEventData.InputButton.Middle: break;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    void TrySell(ItemData data, int count) {
        if (!_inventoryAsset.Remove(data, count)) return;
        _moneyManager.CurrentMoney += data.Price * count;
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

}