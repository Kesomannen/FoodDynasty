using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Topping Item")]
public class ToppingItemData : InventoryItemData {
    [Header("Topping")]
    [SerializeField] MachineItemData _associatedMachine;
    [HideInInspector] public List<string> InheritedInfo = new() { "Multiplier" };
    
    public MachineItemData AssociatedMachine => _associatedMachine;
    public override InventoryItemType Type => InventoryItemType.Topping;

    public override IEnumerable<(string Name, string Value)> GetInfo() {
        if (_associatedMachine == null) {
            return base.GetInfo();
        }
        
        var inherited = _associatedMachine.GetInfo()
            .Where(info => InheritedInfo.Contains(info.Name));

        return base.GetInfo().Concat(inherited);
    }
}