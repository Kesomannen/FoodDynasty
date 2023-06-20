using UnityEngine;

public abstract class ItemMachineComponent : MonoBehaviour {
    [Header("Item Component")]
    [SerializeField] FilteredItemEvent _triggerEvent;

    protected abstract void OnTriggered(Item item);

    void OnEnable() {
        _triggerEvent.Subscribe(OnTriggered);
    }
    
    void OnDisable() {
        _triggerEvent.Unsubscribe(OnTriggered);
    }
}