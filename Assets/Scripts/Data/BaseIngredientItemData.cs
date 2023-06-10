using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Ingredient Item")]
public class BaseIngredientItemData : InventoryItemData, IPrefabProvider<Item> {
    [Header("Base Ingredient")]
    [SerializeField] Item _prefab;

    public override InventoryItemType Type => InventoryItemType.BaseIngredient;
    public Item Prefab => _prefab;

    public override IEnumerable<(string Name, string Value)> GetInfo() {
        return base.GetInfo().Concat(_prefab.GetInfo());
    }
}