using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Machine")]
public class MachineItemData : InventoryItemData, IInventoryItemPrefab<GridObject> {
    [Header("Machine")]
    [SerializeField] GridObject _prefab;
    [HideInInspector]
    [SerializeField] bool[] _enabledProviders;
    
    IInfoProvider[] _cachedProviders;
    
    public GridObject Prefab => _prefab;
    
    public (IInfoProvider Provider, bool Enabled)[] InfoProviders {
        get {
            if (_cachedProviders == null) {
                RefreshProviders();
            }
            
            return _cachedProviders!.Select((provider, i) => (provider, _enabledProviders[i])).ToArray();
        }
        set {
            _cachedProviders = value.Select(pair => pair.Provider).ToArray();
            _enabledProviders = value.Select(pair => pair.Enabled).ToArray();
        }
    }

    public override IEnumerable<(string Name, string Value)> GetInfo() {
        return base.GetInfo().Concat(InfoProviders
            .Where(provider => provider.Enabled)
            .SelectMany(provider => provider.Provider.GetInfo()));
    }

    public void RefreshProviders() {
        if (_prefab == null) {
            _cachedProviders = Array.Empty<IInfoProvider>();
            return;
        }
        
        _cachedProviders = _prefab.GetComponentsInChildren<IInfoProvider>();
        // Something has happened to the providers, so reset the enabled providers
        if (_enabledProviders.Length != _cachedProviders.Length) {
            _enabledProviders = Enumerable.Repeat(true, _cachedProviders.Length).ToArray();
        }
    }
}