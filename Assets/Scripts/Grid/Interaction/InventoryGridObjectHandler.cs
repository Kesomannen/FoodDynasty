using UnityEngine;

public class InventoryGridObjectHandler : MonoBehaviour {
    [SerializeField] GridObjectBuilder _builder;
    [SerializeField] InventoryAsset _inventory;
    [Space]
    [SerializeField] GameEvent<GridObject> _deleteObjectEvent;
    [SerializeField] GameEvent<InventoryItem> _startBuildingEvent;

    void OnEnable() {
        _deleteObjectEvent.OnRaised += OnGridObjectDeleted;
        _startBuildingEvent.OnRaised += OnItemUsed;
    }
    
    void OnDisable() {
        _deleteObjectEvent.OnRaised -= OnGridObjectDeleted;
        _startBuildingEvent.OnRaised -= OnItemUsed;
    }

    void OnGridObjectDeleted(GridObject gridObject) {
        if (!gridObject.TryGetComponent(out IDataProvider<InventoryItemData> itemDataProvider)) return;
        foreach (var handler in gridObject.GetComponentsInChildren<IOnDeletedHandler>()) {
            handler.OnDeleted(_inventory);
        }
        _inventory.Add(itemDataProvider.Data);
    }

    async void OnItemUsed(InventoryItem item) {
        if (item.Inventory != _inventory) return;

        if (item.Data is not IPrefabProvider<GridObject> prefabProvider) return;
        await _builder.StartPlacing(prefabProvider.Prefab, BeforePlace, AfterPlace);

        void AfterPlace(GridObject obj, GridPlacementResult result) {
            if (result.WasSuccessful) return;
            _inventory.Add(item.Data);
        }
        
        bool BeforePlace() {
            return _inventory.Remove(item.Data);
        }
    }
}