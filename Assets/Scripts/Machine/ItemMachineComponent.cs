using UnityEngine;

public abstract class ItemMachineComponent : MonoBehaviour {
    [SerializeField] LocalEvent<Item> _itemEnteredEvent;
    [SerializeField] Optional<LocalConditional<bool>> _triggerCondition;
    [SerializeField] Optional<ItemFilterGroup> _itemFilter;

    protected abstract void OnItemEntered(Item item);

    void OnEnable() {
        _itemEnteredEvent.AddListener(OnItemEventTriggered);
    }
    
    void OnDisable() {
        _itemEnteredEvent.RemoveListener(OnItemEventTriggered);
    }
    
    void OnItemEventTriggered(Item item) { 
        if (_triggerCondition.Enabled && !_triggerCondition.Value.Check()) return;
        if (_itemFilter.Enabled && !_itemFilter.Value.Check(item)) return;
        OnItemEntered(item);
    }
}