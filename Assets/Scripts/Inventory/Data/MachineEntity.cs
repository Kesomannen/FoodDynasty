using UnityEngine;

public class MachineEntity : Entity, IDataProvider<MachineItemData> {
    [SerializeField] MachineItemData _data;
    
    public MachineItemData Data => _data;
    public override string Name => _data.Name;
    public override string Description => _data.Description;
}