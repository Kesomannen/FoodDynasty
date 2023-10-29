using System;
using System.Collections.Generic;
using System.Linq;
using Dynasty.Library;
using UnityEngine;

namespace Dynasty.UI.Tutorial {

[CreateAssetMenu(menuName = "Tutorial/Shop Mission")]
public class ShopMission : Mission {
    [SerializeField] InventoryAsset _inventory;
    [SerializeField] MissionItem[] _items;
    
    readonly Dictionary<MissionItem, int> _currentAmounts = new();
    
    public override float Goal => _items.Sum(item => item.Amount);
    
    public override void Start() {
        _currentAmounts.Clear();
        _items.ForEach(item => _currentAmounts.Add(item, 0));
        
        _inventory.OnItemsAdded += OnItemsAdded;
        _inventory.OnItemsRemoved += OnItemsRemoved;
    }

    protected override void OnComplete() {
        _inventory.OnItemsAdded -= OnItemsAdded;
        _inventory.OnItemsRemoved -= OnItemsRemoved;
    }

    void OnItemsAdded(ItemData data, int count) {
        ChangeCount(data, count);
    }

    void OnItemsRemoved(ItemData data, int count) {
        ChangeCount(data, -count);
    }

    void ChangeCount(ItemData data, int count) {
        var item = _items.FirstOrDefault(item => item.ItemData == data);
        if (item == null) return;
        
        _currentAmounts[item] += count;
        Progress = _currentAmounts.Sum(pair => Mathf.Min(pair.Value, pair.Key.Amount));
    }

    [Serializable]
    class MissionItem {
        public ItemData ItemData;
        public int Amount;
    }
}

}