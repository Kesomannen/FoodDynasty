using System.Collections.Generic;
using System.Linq;
using Dynasty.Core.Inventory;
using Dynasty.Library;
using UnityEngine;

namespace Dynasty.Food {

[CreateAssetMenu(menuName = "Inventory/Food")]
public class FoodItemData : ItemData, IPrefabProvider<FoodBehaviour> {
    [Header("Base Ingredient")]
    [SerializeField] FoodBehaviour _prefab;

    public override ItemType Type => ItemType.Food;
    public FoodBehaviour Prefab => _prefab;

    public override IEnumerable<EntityInfo> GetInfo() {
        return base.GetInfo().Concat(_prefab.GetInfo());
    }
}

}