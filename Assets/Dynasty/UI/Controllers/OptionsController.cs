using System.Linq;
using Dynasty.Core.Grid;
using Dynasty.Core.Inventory;
using Dynasty.Food.Instance;
using Dynasty.Machine.Components;
using UnityEngine;

namespace Dynasty.UI.Controllers {

public class OptionsController : MonoBehaviour {
    [SerializeField] InventoryAsset _inventory;
    [SerializeField] GridManager _grid;
    [SerializeField] InventoryGridObjectHandler _gridObjectHandler;
    
    public void ClearAllFood() {
        var manager = FoodManager.Singleton;
        if (manager == null) return;
        
        foreach (var food in manager.GetAllFood().ToArray()) {
            food.Dispose();
        }
    }

    public void EmptyAllMachines() {
        foreach (var supply in FindObjectsOfType<Supply>()) {
            supply.Empty(_inventory);
        }
    }

    public void DeleteAllMachines() {
        var gridObjects = _grid.GridObjects.ToArray();
        foreach (var gridObject in gridObjects) {
            if (!_gridObjectHandler.RegisterDeletion(gridObject)) return;
            _grid.TryRemove(gridObject);
            Destroy(gridObject.gameObject);
        }
    }
}

}