using System;
using System.Collections.Generic;
using System.Linq;
using Dynasty.Core.Inventory;
using Dynasty.Core.Tooltip;
using Dynasty.Library.Events;
using Dynasty.Library.Extensions;
using Dynasty.UI.Components;
using Dynasty.UI.Controls;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Dynasty.UI.Controllers {

public class ShopController : MonoBehaviour {
    [Header("Shop")]
    [SerializeField] Transform _itemParent;
    [SerializeField] Container<ItemData> _itemPrefab;
    [SerializeField] GameEvent<Item> _onItemPurchased;
    [SerializeField] ItemDataBuyControl _buyControl;
    [SerializeField] MoneyManager _moneyManager;
    [SerializeField] ListEvent<ItemData> _content;
    [SerializeField] TooltipData<ItemData> _tooltipData;
    [SerializeField] UnityEvent _onBuy;

    readonly List<(Interactable interactable, Container<ItemData> container)> _items = new();

    void Awake() {
        _content.AddListener(AddItem, RemoveItem);
    }

    void OnEnable() {
        _moneyManager.OnMoneyChanged += OnMoneyChanged;
        OnMoneyChanged(0, _moneyManager.CurrentMoney);
    }
    
    void OnDisable() {
        _moneyManager.OnMoneyChanged -= OnMoneyChanged;
    }

    void OnDestroy() {
        _content.RemoveListener(AddItem, RemoveItem);

        while (_items.Count > 0) {
            var (interactable, container) = _items[0];
            interactable.OnClicked -= OnItemClicked;
            _items.Remove((interactable, container));
        }
    }

    void AddItem(ItemData item) {
        var itemContainer = Instantiate(_itemPrefab, _itemParent);
        itemContainer.SetContent(item);
            
        var interactable = itemContainer.GetOrAddComponent<Interactable>();
        interactable.OnClicked += OnItemClicked;
        interactable.OnHovered += OnItemHovered;

        _items.Add((interactable, itemContainer));
    }
    
    void RemoveItem(ItemData item) {
        var (interactable, container) = _items.First(i => i.container.Content == item);
        
        interactable.OnClicked -= OnItemClicked;
        _items.Remove((interactable, container));
        
        Destroy(container.gameObject);
    }

    void TryBuy(ItemData item, int count) {
        var cost = item.Price * count;
        if (_moneyManager.CurrentMoney < cost) return;

        _moneyManager.CurrentMoney -= cost;
        _onItemPurchased.Raise(new Item { Count = count, Data = item });
        
        _onBuy.Invoke();
    }
    
    void OnMoneyChanged(double previous, double current) {
        foreach (var (interactable, container) in _items) {
            interactable.interactable = current >= container.Content.Price;
        }
    }

    void OnItemClicked(Interactable interactable, PointerEventData eventData) {
        var data = GetData(interactable);
        
        switch (eventData.button) {
            case PointerEventData.InputButton.Left:
                TryBuy(data, 1); break;
            case PointerEventData.InputButton.Right:
                _buyControl.Initialize(data, _moneyManager, count => TryBuy(data, count));
                _buyControl.gameObject.SetActive(true);
                break;
            case PointerEventData.InputButton.Middle: break;
            default: throw new ArgumentOutOfRangeException();
        }
    }
    
    void OnItemHovered(Interactable interactable, bool hovered, PointerEventData eventData) {
        _tooltipData.Show(GetData(interactable), hovered);
    }

    ItemData GetData(Interactable interactable) {
        return _items.First(item => item.interactable == interactable).container.Content;
    }
}

}