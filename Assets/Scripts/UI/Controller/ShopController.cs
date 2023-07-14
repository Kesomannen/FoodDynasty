using System;
using System.Collections.Generic;
using System.Linq;
using Dynasty.Library.Classes;
using Dynasty.Library.Events;
using Dynasty.Library.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopController : MonoBehaviour {
    [Header("Shop")]
    [SerializeField] Transform _itemParent;
    [SerializeField] Container<ItemData> _itemPrefab;
    [SerializeField] GameEvent<Item> _onItemPurchased;
    [SerializeField] ItemDataBuyControl _buyControl;
    [SerializeField] ItemData[] _itemData;
    [SerializeField] MoneyManager _moneyManager;
    [Space]
    [SerializeField] bool _descendingOrder;
    [SerializeField] ItemSortingMode _sortingMode;
    [SerializeField] TooltipData<ItemData> _tooltipData;

    readonly List<(Interactable interactable, Container<ItemData> container)> _items = new();

    void Awake() {
        foreach (var item in _itemData.Sorted(_sortingMode, _descendingOrder)) {
            var itemContainer = Instantiate(_itemPrefab, _itemParent);
            itemContainer.SetContent(item);
            
            var interactable = itemContainer.GetOrAddComponent<Interactable>();
            interactable.OnClicked += OnItemClicked;
            interactable.OnHovered += OnItemHovered;

            _items.Add((interactable, itemContainer));
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
        for (var i = 0; i < _items.Count; i++) {
            var (interactable, container) = _items[i];
            interactable.OnClicked -= OnItemClicked;
            _items.Remove((interactable, container));
            i--;
        }
    }

    void TryBuy(ItemData item, int count) {
        var cost = item.Price * count;
        if (_moneyManager.CurrentMoney < cost) return;

        _moneyManager.CurrentMoney -= cost;
        _onItemPurchased.Raise(new Item { Count = count, Data = item });
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