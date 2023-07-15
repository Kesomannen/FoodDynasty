using Dynasty.Food.Instance;
using Dynasty.Machine.Components;
using UnityEngine;

namespace Dynasty.Machine.Internal {

public abstract class FoodMachineComponent : MonoBehaviour, IMachineComponent {
    [SerializeField] FilteredFoodEvent _triggerEvent;

    public FilteredFoodEvent TriggerEvent {
        get => _triggerEvent;
        set => _triggerEvent = value;
    }

    protected abstract void OnTriggered(FoodBehaviour food);

    protected virtual void OnEnable() {
        _triggerEvent.Subscribe(OnTriggered);
    }
    
    protected virtual void OnDisable() {
        _triggerEvent.Unsubscribe(OnTriggered);
    }
    
    public Component Component => this;
}

}