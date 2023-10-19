using System.Collections.Generic;
using System.Linq;
using Dynasty.Library;
using Dynasty.UI.Miscellaneous;
using UnityEngine;

namespace Dynasty.UI.Components {

public class EntityStatus : UIComponent<Entity> {
    [SerializeField] int _statusChildIndexStart;
    [SerializeField] ContainerObjectPool<EntityInfo> _pool;
    [SerializeField] Transform _containerParent;

    readonly Dictionary<IStatusProvider, Container<EntityInfo>[]> _providerToContainer = new();

    public override void SetContent(Entity content) {
        Clear();
        
        var providers = content.GetComponentsInChildren<IStatusProvider>();
        if (providers.Length == 0) return;

        for (var i = 0; i < providers.Length; i++) {
            var provider = providers[i];
            
            provider.OnStatusChanged += OnStatusChanged;
            var containers = provider.GetStatus()
                .Select(status => _pool.Get(status, _containerParent, _statusChildIndexStart + i)).ToArray();

            _providerToContainer.Add(provider, containers);
        }
    }

    void Clear() {
        foreach (var (provider, containers) in _providerToContainer) {
            provider.OnStatusChanged -= OnStatusChanged;
            foreach (var container in containers) {
                container.Dispose();
            }
        }
        
        _providerToContainer.Clear();
    }

    void OnStatusChanged(IStatusProvider provider) {
        var statuses = provider.GetStatus().ToArray();
        var containers = _providerToContainer[provider];
        
        for (var i = 0; i < containers.Length; i++) {
            containers[i].SetContent(statuses[i]);
        }
    }
}

}