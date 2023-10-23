using System.Collections.Generic;
using Dynasty.Library;
using Dynasty.Machines;
using Dynasty.UI.Miscellaneous;
using UnityEngine;

namespace Dynasty.UI.Components {

public class RefillerStateController : UpdatingUIComponent<AutoRefiller> {
    [SerializeField] Transform _stateParent;
    [SerializeField] ContainerObjectPool<RefillerState> _refillerStatePool;

    readonly Queue<Container<RefillerState>> _stateContainers = new();
    
    protected override void Subscribe(AutoRefiller content) {
        content.OnStatusChanged += OnStatusChanged;
        RebuildStates();
    }

    protected override void Unsubscribe(AutoRefiller content) {
        content.OnStatusChanged -= OnStatusChanged;
    }

    void OnStatusChanged(IStatusProvider provider) {
        if (Content.States.Count != _stateContainers.Count) {
            RebuildStates();
        }
    }

    void RebuildStates() {
        while (_stateContainers.Count > 0) {
            _stateContainers.Dequeue().Dispose();
        }
        
        foreach (var state in Content.States) {
            var container = _refillerStatePool.Get(state.Value, _stateParent, 1);
            _stateContainers.Enqueue(container);
        }
    }
}

}