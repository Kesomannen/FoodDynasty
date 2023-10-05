using Dynasty.Food.Instance;
using UnityEngine;

namespace Dynasty.Machines {

public class SplitConveyor : FoodMachineComponent {
    [SerializeField] Conveyor[] _conveyors;
    
    int _index;
    
    protected override void OnTriggered(FoodBehaviour food) {
        _index = (_index + 1) % _conveyors.Length;
        for (var i = 0; i < _conveyors.Length; i++) {
            _conveyors[i].enabled = i == _index;
        }
    }
}

}