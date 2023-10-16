using System.Linq;
using Dynasty.Core.Grid;
using Dynasty.Core.Inventory;
using Dynasty.Food;
using Dynasty.Machines;
using UnityEngine;

namespace Dynasty.UI.Controllers {

public class ActionsController : MonoBehaviour {
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
            if (!gridObject.CanMove || !_gridObjectHandler.RegisterDeletion(gridObject)) continue;
            
            _grid.TryRemove(gridObject);
            Destroy(gridObject.gameObject);
        }
    }
}

}