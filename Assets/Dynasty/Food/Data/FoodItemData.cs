using System.Collections.Generic;
using System.Linq;
using Dynasty.Core.Inventory;
using Dynasty.Food.Instance;
using Dynasty.Library.Entity;
using UnityEngine;

namespace Dynasty.Food.Data {

[CreateAssetMenu(menuName = "Inventory/Food")]
public class FoodItemData : ItemData, IPrefabProvider<FoodBehaviour> {
    [Header("Base Ingredient")]
    [SerializeField] FoodBehaviour _prefab;

    public override ItemType Type => ItemType.BaseIngredient;
    public FoodBehaviour Prefab => _prefab;

    public override IEnumerable<EntityInfo> GetInfo() {
        return base.GetInfo().Concat(_prefab.GetInfo());
    }
}

}