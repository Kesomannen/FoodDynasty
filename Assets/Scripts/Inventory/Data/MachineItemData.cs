using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Machine Item")]
public class MachineItemData : InventoryItemData, IPrefabProvider<GridObject> {
    [SerializeField] InventoryItemType _type;
    
    [Header("Machine")]
    [SerializeField] GridObject _prefab;
    [SerializeField] bool[] _enabledProviders;
    
    IInfoProvider[] _cachedProviders;
    
    public override InventoryItemType Type => _type;
    public GridObject Prefab => _prefab;

    public (IInfoProvider Provider, bool Enabled)[] GetInfoProviders() {
        if (_cachedProviders == null) {
            RefreshProviders();
        }

        return _cachedProviders!.Select((provider, i) => (provider, _enabledProviders[i])).ToArray();
    }

    public void SetInfoProviders((IInfoProvider Provider, bool Enabled)[] value) {
        _cachedProviders = value.Select(pair => pair.Provider).ToArray();
        _enabledProviders = value.Select(pair => pair.Enabled).ToArray();
    }

    public override IEnumerable<(string Name, string Value)> GetInfo() {
        return base.GetInfo().Concat(GetInfoProviders()
            .Where(provider => provider.Enabled)
            .SelectMany(provider => provider.Provider.GetInfo()));
    }

    public void RefreshProviders() {
        _cachedProviders = _prefab == null ? Array.Empty<IInfoProvider>() : _prefab.GetComponentsInChildren<IInfoProvider>();

        if (_cachedProviders.Length == _enabledProviders.Length) return;
        Debug.Log("Resetting enabled providers", this);
        _enabledProviders = new bool[_cachedProviders.Length];
    }
}