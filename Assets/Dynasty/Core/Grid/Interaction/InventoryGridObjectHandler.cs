using Dynasty.Core.Data;
using Dynasty.Core.Inventory;
using Dynasty.Library.Events;
using UnityEngine;

namespace Dynasty.Core.Grid {

/// <summary>
/// Handles interaction between <see cref="InventoryAsset"/> and <see cref="GridObject"/>s.
/// </summary>
public class InventoryGridObjectHandler : MonoBehaviour {
    [SerializeField] GridObjectBuilder _builder;
    [SerializeField] InventoryAsset _inventory;
    
    [Space]
    
    [Tooltip("When raised, adds the given object back to the inventory.")]
    [SerializeField] GameEvent<GridObject> _deleteObjectEvent;
    
    [Tooltip("When raised, attempts to start building the given item.")]
    [SerializeField] GameEvent<Item> _startBuildingEvent;

    void OnEnable() {
        _deleteObjectEvent.AddListener(OnGridObjectDeleted);
        _startBuildingEvent.AddListener(OnStartBuilding);
    }
    
    void OnDisable() {
        _deleteObjectEvent.RemoveListener(OnGridObjectDeleted);
        _startBuildingEvent.RemoveListener(OnStartBuilding);
    }

    void OnGridObjectDeleted(GridObject gridObject) {
        if (!gridObject.TryGetComponent(out IDataProvider<MachineItemData> machineProvider)) return;
        foreach (var handler in gridObject.GetComponentsInChildren<IOnDeletedHandler>()) {
            handler.OnDeleted(_inventory);
        }
        _inventory.Add(machineProvider.Data);
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

}