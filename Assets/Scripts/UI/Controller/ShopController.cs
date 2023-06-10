using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour {
    [Header("Shop")]
    [SerializeField] Transform _shopItemParent;
    [SerializeField] UIContainer<InventoryItemData> _shopItemPrefab;
    [SerializeField] InventoryItemData[] _itemData;
    [SerializeField] MoneyManager _moneyManager;
    [SerializeField] GameEvent<InventoryItemData> _onItemPurchased;

    [Header("Tooltip")]
    [SerializeField] Transform _tooltipLockPoint;
    [SerializeField] TooltipLockAxis _tooltipLockAxis;
    [SerializeField] GameEvent<TooltipParams> _showTooltipEvent;
    [SerializeField] GenericGameEvent _hideTooltipEvent;

    readonly List<UIContainer<InventoryItemData>> _itemContainers = new();

    void OnEnable() {
        _moneyManager.OnMoneyChanged += OnMoneyChanged;
    }

    void Start() {
        foreach (var item in _itemData) {
            var itemContainer = Instantiate(_shopItemPrefab, _shopItemParent);
            itemContainer.OnClicked += OnItemClicked;
            itemContainer.OnHovered += OnItemHovered;
            itemContainer.SetContent(item);
            
            _itemContainers.Add(itemContainer);
        }
    }

    void OnDisable() {
        _moneyManager.OnMoneyChanged -= OnMoneyChanged;
    }

    void OnDestroy() {
        for (var i = 0; i < _itemContainers.Count; i++) {
            var itemContainer = _itemContainers[i];
            itemContainer.OnClicked -= OnItemClicked;
            _itemContainers.Remove(itemContainer);
            i--;
        }
    }
    
    void OnMoneyChanged(double previous, double current) {
        foreach (var itemContainer in _itemContainers) {
            itemContainer.Interactable = current >= itemContainer.Content.Price;
        }
    }

    void OnItemClicked(UIContainer<InventoryItemData> itemContainer) {
        var item = itemContainer.Content;
        if (item.Price > _moneyManager.CurrentMoney) return;
        
        _moneyManager.CurrentMoney -= item.Price;
        _onItemPurchased.Raise(item);
    }
    
    void OnItemHovered(UIContainer<InventoryItemData> itemContainer, bool hovered) {
        if (hovered) {
            var parameters = new TooltipParams {
                Content = itemContainer.Content,
                LockAxis = _tooltipLockAxis,
                LockPoint = _tooltipLockPoint
            };
            _showTooltipEvent.Raise(parameters);
        } else {
            _hideTooltipEvent.Raise();
        }
    }
}