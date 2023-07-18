using Dynasty.Core.Data;
using Dynasty.Library.Entity;
using UnityEngine;

namespace Dynasty.Core.Inventory {

/// <summary>
/// Links a machine to its item data.
/// </summary>
public class MachineEntity : Entity, IDataProvider<MachineItemData> {
    [SerializeField] MachineItemData _data;
    
    /// <summary>
    /// The data for this machine.
    /// </summary>
    public MachineItemData Data {
        get => _data;
        set => _data = value;
    }
    
    public override string Name => _data.Name;
    public override string Description => _data.Description;
    public override Sprite Icon => _data.Icon;
}

}