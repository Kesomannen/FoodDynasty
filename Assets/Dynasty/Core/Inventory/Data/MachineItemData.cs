using System;
using System.Collections.Generic;
using System.Linq;
using Dynasty.Library;
using Dynasty.Core.Grid;
using UnityEngine;

namespace Dynasty.Core.Inventory {

/// <summary>
/// Represents an item that can be placed on the grid.
/// </summary>
[CreateAssetMenu(menuName = "Inventory/Machine Item")]
public class MachineItemData : ItemData, IPrefabProvider<GridObject> {
    [SerializeField] ItemType _type;
    
    [Header("Machine")]
    [SerializeField] GridObject _prefab;
    
    /// <summary>
    /// Determines which info providers to use.
    /// </summary>
    [HideInInspector]
    [SerializeField] bool[] _enabledProviders;
     

    /// <summary>
    /// Info providers found on the prefab.
    /// </summary>
    /// <remarks>Cached to avoid excessive GetComponentsInChildren calls.</remarks>
    IInfoProvider[] _cachedInfoProviders;
    
    public override ItemType Type => _type;

    public void SetType(ItemType value) {
        _type = value;
    }

    /// <summary>
    /// The prefab for this machine, which can be instantiated on the grid.
    /// </summary>
    public GridObject Prefab {
        get => _prefab;
        set => _prefab = value;
    }

    /// <summary>
    /// Returns the info providers found on the prefab, accompanied by their status.
    /// </summary>
    public (IInfoProvider Provider, bool Enabled)[] GetInfoProviders() {
        if (_cachedInfoProviders == null) {
            RefreshProviders();
        }

        return _cachedInfoProviders!.Select((provider, i) => (provider, _enabledProviders[i])).ToArray();
    }

    /// <summary>
    /// Sets the info providers found on the prefab, accompanied by their status.
    /// </summary>
    public void SetInfoProviders((IInfoProvider Provider, bool Enabled)[] value) {
        _cachedInfoProviders = value.Select(pair => pair.Provider).ToArray();
        _enabledProviders = value.Select(pair => pair.Enabled).ToArray();
    }

    public override IEnumerable<EntityInfo> GetInfo() {
        return base.GetInfo().Concat(GetInfoProviders()
            .Where(provider => provider.Enabled)
            .SelectMany(provider => provider.Provider.GetInfo()));
    }

    /// <summary>
    /// Refreshes the info providers found on the prefab.
    /// </summary>
    /// <remarks>Slightly expensive, avoid calling in game loops.</remarks>
    public void RefreshProviders() {
        _cachedInfoProviders = _prefab == null ? Array.Empty<IInfoProvider>() : _prefab.GetComponentsInChildren<IInfoProvider>();

        // If the number of providers has changed, reset the enabled array.
        if (_enabledProviders != null && _cachedInfoProviders.Length == _enabledProviders.Length) return;
        _enabledProviders = Enumerable.Repeat(true, _cachedInfoProviders.Length).ToArray();
    }

    void Reset() {
        RefreshProviders();
    }
}

}