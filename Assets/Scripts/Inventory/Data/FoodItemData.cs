using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Food")]
public class FoodItemData : InventoryItemData, IPrefabProvider<Food> {
    [Header("Base Ingredient")]
    [SerializeField] Food _prefab;

    public override InventoryItemType Type => InventoryItemType.BaseIngredient;
    public Food Prefab => _prefab;

    public override IEnumerable<(string Name, string Value)> GetInfo() {
        return base.GetInfo().Concat(_prefab.GetInfo());
    }
}