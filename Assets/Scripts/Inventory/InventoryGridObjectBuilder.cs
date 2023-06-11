using Unity.VisualScripting;
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
        await _builder.StartPlacing(prefabProvider.Prefab, BeforePlace, AfterPlace);

        void AfterPlace(GridObject obj, GridPlacementResult result) {
            if (result.Succeeded) return;
            item.Inventory.Add(item.Data);
        }
        
        bool BeforePlace() {
            return item.Inventory.Remove(item.Data);
        }
    }
}