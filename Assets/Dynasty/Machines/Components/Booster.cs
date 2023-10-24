using System.Collections.Generic;
using System.Linq;
using Dynasty.Library;
using Dynasty.Library.Classes;
using UnityEngine;

namespace Dynasty.Machines {

public class Booster : MachineModifier<IBoostable> {
    [SerializeField] Modifier _modifier;
    
    public Modifier Modifier {
        get => _modifier;
        set => _modifier = value;
    }

    protected override void OnAdded(IBoostable boostable) {
        boostable.Modifier += _modifier;
    }
    
    protected override void OnRemoved(IBoostable boostable) {
        boostable.Modifier -= _modifier;
    }

    public override IEnumerable<EntityInfo> GetInfo() {
        return base.GetInfo()
            .Append(new EntityInfo("Boost", _modifier.ToString()));
    }
}

}