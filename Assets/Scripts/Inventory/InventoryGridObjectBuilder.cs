using UnityEngine;

public class InventoryGridObjectBuilder : MonoBehaviour {
    [SerializeField] GridObjectBuilder _builder;
    [SerializeField] GameEvent<InventoryItem> _onItemUsed;

    void OnEnable() {
        _onItemUsed.OnRaised += OnItemUsed;
    }
    
    void OnDisable() {
        _onItemUsed.OnRaised -= OnItemUsed;
    }

    async void OnItemUsed(InventoryItem item) {
        _builder.Cancel();
        
        if (item.Data is not IPrefabProvider<GridObject> prefabProvider) return;
        await _builder.StartPlacing(prefabProvider.Prefab, GridObjectPlaced);
        
        void GridObjectPlaced(GridObject gridObject) {
            if (item.Owner.Remove(item.Data)) return;
            _builder.Cancel();
        }
    }
}