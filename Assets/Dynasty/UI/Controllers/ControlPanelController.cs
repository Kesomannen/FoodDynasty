using System;
using System.Linq;
using Dynasty.Grid;
using Dynasty;
using Dynasty.Food;
using Dynasty.Library;
using Dynasty.Machines;
using Google.MaterialDesign.Icons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dynasty.UI.Controllers {

public class ControlPanelController : MonoBehaviour {
    [SerializeField] InventoryAsset _inventory;
    [SerializeField] GridManager _grid;
    [SerializeField] InventoryGridObjectHandler _gridObjectHandler;
    [SerializeField] Slider _maxFoodBar;
    [SerializeField] TMP_Text _maxFoodText;
    [Space]
    [SerializeField] FoodCamera _foodCamera;
    [SerializeField] MaterialIcon _foodCameraIcon;
    [SerializeField] string _enabledFoodCameraUnicode;
    
    string _disabledFoodCameraUnicode;

    void Awake() {
        _disabledFoodCameraUnicode = _foodCameraIcon.iconUnicode;
    }

    void OnEnable() {
        if (FoodManager.Singleton == null) return;
        
        FoodManager.Singleton.OnFoodChanged += UpdateMaxFood;
        _maxFoodBar.maxValue = FoodManager.Singleton.MaxFood;
        UpdateMaxFood();
    }
    
    void OnDisable() {
        if (FoodManager.Singleton == null) return;
        FoodManager.Singleton.OnFoodChanged -= UpdateMaxFood;
    }
    
    void UpdateMaxFood() {
        _maxFoodBar.value = FoodManager.Singleton.FoodCount;
        _maxFoodText.text = $"Food: {_maxFoodBar.value}/{_maxFoodBar.maxValue}";
    }

    public void ToggleFoodCamera() {
        _foodCamera.IsActive = !_foodCamera.IsActive;
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
        var gridObjects = _grid.GridObjects.ToArray();
        foreach (var gridObject in gridObjects) {
            if (!gridObject.CanMove || !_gridObjectHandler.RegisterDeletion(gridObject)) continue;
            
            _grid.TryRemove(gridObject);
            Destroy(gridObject.gameObject);
        }
    }
}

}