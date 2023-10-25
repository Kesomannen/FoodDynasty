using System;
using System.Collections.Generic;
using Dynasty.Library;
using UnityEngine;

namespace Dynasty.Core.Inventory {

/// <summary>
/// Manages a collection of items.
/// </summary>
[CreateAssetMenu(menuName = "Inventory/Inventory Asset")]
public class InventoryAsset : MonoScriptable {
    [Tooltip("When raised, adds the given item to the inventory.")]
    [SerializeField] GameEvent<Item> _addItemEvent;

    readonly Dictionary<ItemData, Item> _items = new();
    
    /// <summary>
    /// The items in the inventory.
    /// </summary>
    public IEnumerable<Item> Items => _items.Values;
    
    /// <summary>
    /// Raised when an item's count changes, is added, or is removed.
    /// </summary>
    public event Action<Item> OnItemChanged;

    public override void OnAwake() {
        _items.Clear();
        _addItemEvent.AddListener(Add);
    }

    public override void OnDestroy() {
        _addItemEvent.RemoveListener(Add);
    }

    bool TryGet(ItemData data, out Item item) {
        return _items.TryGetValue(data, out item);
    }
    
    /// <summary>
    /// Gets the count of the given item data.
    /// </summary>
    public int GetCount(ItemData data) {
        return TryGet(data, out var item) ? item.Count : 0;
    }

    /// <summary>
    /// Adds the given item to the inventory.
    /// </summary>
    public void Add(Item item) {
        Add(item.Data, item.Count);
    }
    
    /// <summary>
    /// Adds the given item data to the inventory, with the given count.
    /// </summary>
    public void Add(ItemData data, int count = 1) {
        var item = GetOrAdd(data);
        item.Count += count;
        _items[data] = item;
        
        OnItemChanged?.Invoke(item);
    }

    /// <summary>
    /// Removes a number of the given item data from the inventory. If there are not enough items, nothing is removed.
    /// </summary>
    /// <returns>Whether or not there were enough items to remove.</returns>
    public bool Remove(ItemData data, int count = 1) {
        var item = GetOrAdd(data);
        if (item.Count < count) return false;
        
        item.Count -= count;
        
        if (item.Count <= 0) _items.Remove(data);
        else _items[data] = item;
        
        OnItemChanged?.Invoke(item);
        return true;
    }
    
    public Item GetOrAdd(ItemData data) {
        if (TryGet(data, out var item)) return item;
        return _items[data] = new Item {
            Count = 0,
            Data = data,
            Inventory = this
        };
    }
}

}