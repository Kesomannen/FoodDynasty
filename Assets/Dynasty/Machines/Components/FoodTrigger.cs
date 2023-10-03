using Dynasty.Food.Instance;
using Dynasty.Machines;
using UnityEngine;

namespace Dynasty.Machines {

public class FoodTrigger : MonoBehaviour, IMachineComponent {
    [SerializeField] FilteredFoodEvent _onTriggered;

    public FilteredFoodEvent TriggerEvent {
        get => _onTriggered;
        set => _onTriggered = value;
    }

    void OnTriggerEnter(Collider other) {
        if (!other.TryGetComponent(out FoodBehaviour food)) return;
        _onTriggered.Raise(food);
    }
    
    public Component Component => this;
}

}