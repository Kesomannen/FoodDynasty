using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Food")]
public class FoodItemData : ItemData, IPrefabProvider<Food> {
    [Header("Base Ingredient")]
    [SerializeField] Food _prefab;

    public override ItemType Type => ItemType.BaseIngredient;
    public Food Prefab => _prefab;

    public override IEnumerable<EntityInfo> GetInfo() {
        return base.GetInfo().Concat(_prefab.GetInfo());
    }
}