using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopController : MonoBehaviour {
    [Header("Shop")]
    [SerializeField] Transform _itemParent;
    [SerializeField] InteractableContainer<InventoryItemData> _itemPrefab;
    [SerializeField] GameEvent<InventoryItem> _onItemPurchased;
    [SerializeField] InventoryItemData[] _itemData;
    [Space]
    [SerializeField] MoneyManager _moneyManager;
    [SerializeField] TooltipData<InventoryItemData> _tooltipData;

    readonly List<InteractableContainer<InventoryItemData>> _itemContainers = new();

    void Awake() {
        foreach (var item in _itemData) {
            var itemContainer = Instantiate(_itemPrefab, _itemParent);
            itemContainer.OnClicked += OnItemClicked;
            itemContainer.OnHovered += OnItemHovered;
            itemContainer.SetContent(item);
            
            _itemContainers.Add(itemContainer);
        }
    }

    void OnEnable() {
        _moneyManager.OnMoneyChanged += OnMoneyChanged;
        OnMoneyChanged(0, _moneyManager.CurrentMoney);
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

    bool Buy(InventoryItemData item, int count) {
        var cost = item.Price * count;
        if (_moneyManager.CurrentMoney < cost) return false;
        
        _moneyManager.CurrentMoney -= cost;
        _onItemPurchased.Raise(new InventoryItem { Count = count, Data = item });
        return true;
    }
    
    void OnMoneyChanged(double previous, double current) {
        foreach (var itemContainer in _itemContainers) {
            itemContainer.interactable = current >= itemContainer.Content.Price;
        }
    }

    void OnItemClicked(InteractableContainer<InventoryItemData> itemContainer, PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        Buy(itemContainer.Content, 1);
    }
    
    void OnItemHovered(InteractableContainer<InventoryItemData> itemContainer, bool hovered, PointerEventData eventData) {
        _tooltipData.Show(itemContainer.Content, hovered);
    }
}