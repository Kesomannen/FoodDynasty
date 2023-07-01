using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Topping Item")]
public class ToppingItemData : ItemData {
    [Header("Topping")]
    [SerializeField] MachineItemData _associatedMachine;
    [HideInInspector] public List<string> InheritedInfo = new() { "Multiplier" };
    
    public MachineItemData AssociatedMachine {
        get => _associatedMachine;
        set => _associatedMachine = value;
    }

    public override ItemType Type => ItemType.Topping;

    public override IEnumerable<(string Name, string Value)> GetInfo() {
        if (_associatedMachine == null) {
            return base.GetInfo();
        }
        
        var inherited = _associatedMachine.GetInfo()
            .Where(info => InheritedInfo.Contains(info.Name));

        return base.GetInfo().Concat(inherited);
    }
}