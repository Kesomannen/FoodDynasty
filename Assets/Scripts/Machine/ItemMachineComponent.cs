using UnityEngine;

public abstract class ItemMachineComponent : MonoBehaviour {
    [SerializeField] LocalEvent<Item> _itemEnteredEvent;

    protected abstract void OnItemEntered(Item item);
    
    void OnEnable() {
        _itemEnteredEvent.AddListener(OnItemEntered);
    }
    
    void OnDisable() {
        _itemEnteredEvent.RemoveListener(OnItemEntered);
    }
}