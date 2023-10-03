using System.Collections.Generic;
using System.Linq;
using Dynasty.Library;
using UnityEngine;

namespace Dynasty.Core.Inventory {

/// <summary>
/// Represents a topping item that's applied to food.
/// </summary>
[CreateAssetMenu(menuName = "Inventory/Topping Item")]
public class ToppingItemData : ItemData {
    [Header("Topping")]
    [Tooltip("The machine that this topping is associated with. Used to inherit info from the machine.")]
    [SerializeField] MachineItemData _associatedMachine;
    
    /// <summary>
    /// List of info names which are inherited from <see cref="AssociatedMachine"/>.
    /// </summary>
    [HideInInspector] public List<string> InheritedInfo = new() { "Multiplier" };
    
    /// <summary>
    /// The machine that this topping is associated with.
    /// </summary>
    public MachineItemData AssociatedMachine {
        get => _associatedMachine;
        set => _associatedMachine = value;
    }

    public override ItemType Type => ItemType.Topping;

    public override IEnumerable<EntityInfo> GetInfo() {
        if (_associatedMachine == null) {
            return base.GetInfo();
        }
        
        var inherited = _associatedMachine.GetInfo()
            .Where(info => InheritedInfo.Contains(info.Name));

        return base.GetInfo().Concat(inherited);
    }
}

}