using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Inventory Asset")]
public class InventoryAsset : MonoScriptable {
    [SerializeField] GameEvent<InventoryItem> _addItemEvent;

    readonly Dictionary<InventoryItemData, InventoryItem> _items = new();
    public IEnumerable<InventoryItem> Items => _items.Values;
    
    public event Action<InventoryItem> OnItemChanged;

    public override void OnAwake() {
        _addItemEvent.OnRaised += Add;
    }

    public override void OnDestroy() {
        _addItemEvent.OnRaised -= Add;
    }

    public bool TryGet(InventoryItemData data, out InventoryItem item) {
        return _items.TryGetValue(data, out item);
    }
    
    public int GetCount(InventoryItemData data) {
        return TryGet(data, out var item) ? item.Count : 0;
    }

    public void Add(InventoryItem item) {
        Add(item.Data, item.Count);
    }
    
    public void Add(InventoryItemData data, int count = 1) {
        var item = GetOrAdd(data);
        item.Count += count;
        _items[data] = item;
        
        OnItemChanged?.Invoke(item);
    }
    
    public bool Remove(InventoryItem item) {
        return Remove(item.Data, item.Count);
    }

    public bool Remove(InventoryItemData data, int count = 1) {
        var item = GetOrAdd(data);
        if (item.Count < count) return false;
        
        item.Count -= count;
        
        if (item.Count <= 0) _items.Remove(data);
        else _items[data] = item;
        
        OnItemChanged?.Invoke(item);
        return true;
    }
    
    InventoryItem GetOrAdd(InventoryItemData data) {
        if (TryGet(data, out var item)) return item;
        return _items[data] = new InventoryItem {
            Count = 0,
            Data = data,
            Inventory = this
        };
    }
}