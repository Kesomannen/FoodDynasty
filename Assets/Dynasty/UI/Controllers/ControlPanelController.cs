using System.Linq;
using Dynasty.Grid;
using Dynasty.Food;
using Dynasty.Library;
using Dynasty.Machines;
using Google.MaterialDesign.Icons;
using UnityEngine;
using UnityEngine.UI;

namespace Dynasty.UI.Controllers {

public class ControlPanelController : MonoBehaviour {
    [SerializeField] InventoryAsset _inventory;
    [SerializeField] GridManager _grid;
    [SerializeField] InventoryGridObjectHandler _gridObjectHandler;
    [SerializeField] GameEvent<PopupData> _showPopupEvent;
    [Space]
    [SerializeField] FoodCamera _foodCamera;
    [SerializeField] MaterialIcon _foodCameraIcon;
    [SerializeField] string _enabledFoodCameraUnicode;
    
    string _disabledFoodCameraUnicode;

    void Awake() {
        _disabledFoodCameraUnicode = _foodCameraIcon.iconUnicode;
        
        _foodCamera.OnAvailabilityChanged += _ => UpdateFoodCameraIcon();
    }

    void Start() {
        UpdateFoodCameraIcon();
    }

    public void ToggleFoodCamera() {
        _foodCamera.IsActive = !_foodCamera.IsActive;
        UpdateFoodCameraIcon();
    }

    void UpdateFoodCameraIcon() {
        _foodCameraIcon.GetComponent<Selectable>().interactable = _foodCamera.IsAvailable;
        _foodCameraIcon.iconUnicode = _foodCamera.IsActive ? _enabledFoodCameraUnicode : _disabledFoodCameraUnicode;
    }

    public void ClearAllFood() {
        FoodManager.Singleton.Food.ToArray()
            .ForEach(food => food.Dispose());
    }

    public void EmptyAllMachines() {
        _grid.GridObjects
            .SelectMany(obj => obj.GetComponentsInChildren<Supply>())
            .ForEach(supply => supply.Empty(_inventory));
    }

    public void DeleteAllMachines() {
        _showPopupEvent.Raise(new PopupData {
            Header = "Delete All Machines?",
            Actions = new[] {
                PopupAction.Negative("Delete", () => {
                    var gridObjects = _grid.GridObjects.ToArray();
                    foreach (var gridObject in gridObjects) {
                        if (!gridObject.CanMove || !_gridObjectHandler.RegisterDeletion(gridObject)) continue;
            
                        _grid.TryRemove(gridObject);
                        Destroy(gridObject.gameObject);
                    }
                }),
                PopupAction.Neutral("Cancel"),
            }
        });
    }
}

}