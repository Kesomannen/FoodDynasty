using UnityEngine;

public class InventoryGridObjectHandler : MonoBehaviour {
    [SerializeField] GridObjectBuilder _builder;
    [SerializeField] InventoryAsset _inventory;
    [Space]
    [SerializeField] GameEvent<GridObject> _deleteObjectEvent;
    [SerializeField] GameEvent<Item> _startBuildingEvent;

    void OnEnable() {
        _deleteObjectEvent.OnRaised += OnGridObjectDeleted;
        _startBuildingEvent.OnRaised += OnStartBuilding;
    }
    
    void OnDisable() {
        _deleteObjectEvent.OnRaised -= OnGridObjectDeleted;
        _startBuildingEvent.OnRaised -= OnStartBuilding;
    }

    void OnGridObjectDeleted(GridObject gridObject) {
        if (!gridObject.TryGetComponent(out IDataProvider<ItemData> itemDataProvider)) return;
        foreach (var handler in gridObject.GetComponentsInChildren<IOnDeletedHandler>()) {
            handler.OnDeleted(_inventory);
        }
        _inventory.Add(itemDataProvider.Data);
    }

    async void OnStartBuilding(Item item) {
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