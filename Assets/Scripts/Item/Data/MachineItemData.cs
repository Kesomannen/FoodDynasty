using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Machine Item")]
public class MachineItemData : ItemData, IPrefabProvider<GridObject> {
    [SerializeField] ItemType _type;
    
    [Header("Machine")]
    [SerializeField] GridObject _prefab;
    
    [HideInInspector]
    [SerializeField] bool[] _enabledProviders;
     
    IInfoProvider[] _cachedInfoProviders;
    
    public override ItemType Type => _type;

    public void SetType(ItemType value) {
        _type = value;
    }

    public GridObject Prefab {
        get => _prefab;
        set => _prefab = value;
    }

    public (IInfoProvider Provider, bool Enabled)[] GetInfoProviders() {
        if (_cachedInfoProviders == null) {
            RefreshProviders();
        }

        return _cachedInfoProviders!.Select((provider, i) => (provider, _enabledProviders[i])).ToArray();
    }

    public void SetInfoProviders((IInfoProvider Provider, bool Enabled)[] value) {
        _cachedInfoProviders = value.Select(pair => pair.Provider).ToArray();
        _enabledProviders = value.Select(pair => pair.Enabled).ToArray();
    }

    public override IEnumerable<EntityInfo> GetInfo() {
        return base.GetInfo().Concat(GetInfoProviders()
            .Where(provider => provider.Enabled)
            .SelectMany(provider => provider.Provider.GetInfo()));
    }

    public void RefreshProviders() {
        _cachedInfoProviders = _prefab == null ? Array.Empty<IInfoProvider>() : _prefab.GetComponentsInChildren<IInfoProvider>();

        if (_enabledProviders != null && _cachedInfoProviders.Length == _enabledProviders.Length) return;
        _enabledProviders = new bool[_cachedInfoProviders.Length];
    }

    void Reset() {
        RefreshProviders();
    }
}