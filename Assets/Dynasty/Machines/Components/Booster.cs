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
}

}