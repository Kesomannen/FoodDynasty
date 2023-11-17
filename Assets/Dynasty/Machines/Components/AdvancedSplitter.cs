using System;
using UnityEngine;

namespace Dynasty.Machines {

public class AdvancedSplitter : MonoBehaviour, IMachineComponent {
    [SerializeField] Port[] _ports;

    void Start() {
        foreach (var port in _ports) {
            port.TriggerEvent.Subscribe(food => {
                var outputPort = _ports[port.OutputPortIndex];
                food.transform.position = outputPort.CurrentOutput.position;
                outputPort.AdvanceIndex();
            });
        }
    }

    [Serializable]
    class Port {
        public Transform[] Outputs;
        public FilteredFoodEvent TriggerEvent;
        public int OutputPortIndex;

        [NonSerialized]
        public int CurrentIndex;
        
        public void AdvanceIndex() {
            CurrentIndex = (CurrentIndex + 1) % Outputs.Length;
        }
        
        public Transform CurrentOutput => Outputs[CurrentIndex];
    }

    public Component Component => this;
}

}