using UnityEngine;

public abstract class ItemMachineComponent : MonoBehaviour {
    [Header("Item Component")]
    [SerializeField] Event<Item> _triggerEvent;
    [SerializeField] Optional<CheckEvent<bool>> _condition;
    [SerializeField] Optional<ItemFilterGroup> _itemFilter;

    protected abstract void OnTriggered(Item item);

    void OnEnable() {
        _triggerEvent.OnRaised += Trigger;
    }
    
    void OnDisable() {
        _triggerEvent.OnRaised -= Trigger;
    }

    void Trigger(Item item) { 
        if (_condition.Enabled && !_condition.Value.Check()) return;
        if (_itemFilter.Enabled && !_itemFilter.Value.Check(item)) return;
        OnTriggered(item);
    }
}