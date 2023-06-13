using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityStatus : UIComponent<Entity> {
    [SerializeField] int _statusChildIndexStart;
    [SerializeField] InfoContainer _containerPrefab;
    [SerializeField] Transform _containerParent;

    readonly Dictionary<IStatusProvider, InfoContainer[]> _providerToContainers = new();
    readonly List<InfoContainer> _unUsedContainers = new();

    public override void SetContent(Entity content) {
        if (_providerToContainers != null) {
            ClearProviders();
        }
        
        var providers = content.GetComponents<IStatusProvider>();
        if (providers.Length != 0) {
            foreach (var provider in providers) {
                provider.OnStatusChanged += OnStatusChanged;
                _providerToContainers!.Add(provider, GetContainers(provider));
            }   
        }
         
        ClearContainers();
    }

    InfoContainer[] GetContainers(IStatusProvider provider) {
        var statuses = provider.GetStatus().ToArray();
        var containers = new InfoContainer[statuses.Length];

        for (var i = 0; i < statuses.Length; i++) {
            if (_unUsedContainers.Count == 0) {
                var obj = Instantiate(_containerPrefab, _containerParent);
                obj.transform.SetSiblingIndex(_statusChildIndexStart);
                _unUsedContainers.Add(obj);
            }

            containers[i] = _unUsedContainers[0];
            containers[i].SetContent(statuses[i]);
            _unUsedContainers.RemoveAt(0);
        }

        return containers;
    }

    void ClearProviders() {
        foreach (var (provider, containers) in _providerToContainers) {
            provider.OnStatusChanged -= OnStatusChanged;
            _unUsedContainers.AddRange(containers);
        }
        _providerToContainers.Clear();
    }
    
    void ClearContainers() {
        foreach (var container in _unUsedContainers) {
            Destroy(container.gameObject);
        }
        _unUsedContainers.Clear();
    }

    void OnDisable() {
        ClearProviders();
        ClearContainers();
    }

    void OnStatusChanged(IStatusProvider provider) {
        var statuses = provider.GetStatus().ToArray();
        var containers = _providerToContainers[provider];
        
        for (var i = 0; i < containers.Length; i++) {
            containers[i].SetContent(statuses[i]);
        }
    }
}