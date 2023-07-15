using System.Collections.Generic;
using System.Linq;
using Dynasty.Library.Entity;
using Dynasty.UI.Miscellaneous;
using UnityEngine;

namespace Dynasty.UI.Components {

public class EntityInfo<T> : UIComponent<T> where T : IInfoProvider {
    [SerializeField] int _infoChildIndexStart;
    [SerializeField] ContainerObjectPool<EntityInfo> _pool;
    [SerializeField] Transform _containerParent;

    readonly Queue<Container<EntityInfo>> _currentContainers = new();

    public override void SetContent(T content) {
        Clear();
        
        var containers = content.GetInfo()
            .Select((info, i) => _pool.Get(info, _containerParent, _infoChildIndexStart + i)).ToArray();

        foreach (var container in containers) {
            _currentContainers.Enqueue(container);
        }
    }

    void Clear() {
        while (_currentContainers.Count > 0) {
            _currentContainers.Dequeue().Dispose();
        }
    }
}

}