using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Inventory Asset")]
public class InventoryAsset : MonoScriptable {
    [SerializeField] GameEvent<Item> _addItemEvent;

    readonly Dictionary<ItemData, Item> _items = new();
    public IEnumerable<Item> Items => _items.Values;
    
    public event Action<Item> OnItemChanged;

    public override void OnAwake() {
        _items.Clear();
        _addItemEvent.OnRaised += Add;
    }

    public override void OnDestroy() {
        _addItemEvent.OnRaised -= Add;
    }

    public bool TryGet(ItemData data, out Item item) {
        return _items.TryGetValue(data, out item);
    }
    
    public int GetCount(ItemData data) {
        return TryGet(data, out var item) ? item.Count : 0;
    }

    public void Add(Item item) {
        Add(item.Data, item.Count);
    }
    
    public void Add(ItemData data, int count = 1) {
        var item = GetOrAdd(data);
        item.Count += count;
        _items[data] = item;
        
        OnItemChanged?.Invoke(item);
    }
    
    public bool Remove(Item item) {
        return Remove(item.Data, item.Count);
    }

    public bool Remove(ItemData data, int count = 1) {
        var item = GetOrAdd(data);
        if (item.Count < count) return false;
        
        item.Count -= count;
        
        if (item.Count <= 0) _items.Remove(data);
        else _items[data] = item;
        
        OnItemChanged?.Invoke(item);
        return true;
    }
    
    Item GetOrAdd(ItemData data) {
        if (TryGet(data, out var item)) return item;
        return _items[data] = new Item {
            Count = 0,
            Data = data,
            Inventory = this
        };
    }
}