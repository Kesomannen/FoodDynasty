using UnityEngine;

public abstract class FoodMachineComponent : MonoBehaviour, IMachineComponent {
    [SerializeField] FilteredItemEvent _triggerEvent;

    protected abstract void OnTriggered(Food food);

    protected virtual void OnEnable() {
        _triggerEvent.Subscribe(OnTriggered);
    }
    
    protected virtual void OnDisable() {
        _triggerEvent.Unsubscribe(OnTriggered);
    }
    
    public Component Component => this;
}