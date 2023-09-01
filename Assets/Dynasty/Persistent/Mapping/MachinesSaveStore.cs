using System;
using System.Linq;
using Dynasty.Core.Grid;
using Dynasty.Core.Inventory;
using Dynasty.Persistent.Core;
using UnityEngine;

namespace Dynasty.Persistent.Mapping {

public class MachinesSaveStore : SaveStore<MachineLoader.Data> {
    [Space]
    [SerializeField] MachineLoader _loader;
    [SerializeField] StartingMachine[] _startingMachines;

    protected override MachineLoader.Data GetDefaultData() {
        return new MachineLoader.Data {
            ItemIds = _startingMachines.Select(machine => _loader.Lookup.GetId(machine.ItemData)).ToArray(),
            Positions = _startingMachines.Select(machine => machine.Position).ToArray(),
            Rotations = _startingMachines.Select(machine => machine.Rotation).ToArray(),
            AdditionalData = _startingMachines.Select(_ => Array.Empty<object>()).ToArray()
        };
    }

    protected override void OnAfterLoad(MachineLoader.Data data) {
        _loader.Load(data);
    }

    protected override MachineLoader.Data GetSaveData() {
        return _loader.Get();
    }
    
    [Serializable]
    struct StartingMachine {
        [SerializeField] MachineItemData _itemData;
        [SerializeField] Vector2Int _position;
        [SerializeField] GridRotation _rotation;
        
        public MachineItemData ItemData => _itemData;
        public SerializableVector2Int Position => new(_position);
        public GridRotation Rotation => _rotation;
    }
}

}