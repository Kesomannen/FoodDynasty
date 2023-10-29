using Dynasty.Library;
using UnityEngine;

namespace Dynasty {

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
    public override string ShortDescription => _data.ShortDescription;
    public override Sprite Icon => _data.Icon;
}

}