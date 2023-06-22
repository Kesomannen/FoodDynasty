using UnityEngine;

public abstract class FoodMachineComponent : MonoBehaviour {
    [Header("Food Component")]
    [SerializeField] FilteredItemEvent _triggerEvent;

    protected abstract void OnTriggered(Food food);

    void OnEnable() {
        _triggerEvent.Subscribe(OnTriggered);
    }
    
    void OnDisable() {
        _triggerEvent.Unsubscribe(OnTriggered);
    }
}