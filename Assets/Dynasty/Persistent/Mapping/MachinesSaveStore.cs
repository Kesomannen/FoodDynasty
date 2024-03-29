﻿using System;
using System.Linq;
using Dynasty.Grid;
using Dynasty;
using UnityEngine;

namespace Dynasty.Persistent {

public class MachinesSaveStore : SaveStore<MachineSaveData> {
    [Space]
    [SerializeField] MachineLoader _loader;
    [SerializeField] StartingMachine[] _startingMachines;

    protected override MachineSaveData DefaultData => new() {
        ItemIds = _startingMachines.Select(machine => _loader.Lookup.GetId(machine.ItemData)).ToArray(),
        Positions = _startingMachines.Select(machine => machine.Position).ToArray(), 
        Rotations = _startingMachines.Select(machine => machine.Rotation).ToArray(),
        AdditionalData = _startingMachines.Select(_ => new MachineSaveData.AdditionalDataItem()).ToArray()
    };
    
    protected override void OnLoad(MachineSaveData data) {
        _loader.Load(data);
    }

    protected override MachineSaveData GetSaveData() {
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