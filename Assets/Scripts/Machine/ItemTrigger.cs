using UnityEngine;

public class ItemTrigger : MonoBehaviour {
    [SerializeField] Optional<ItemFilterGroup> _itemFilter;
    [SerializeField] LocalEvent<Item> _onEnterEvent;

    void OnTriggerEnter(Collider other) {
        if (!other.TryGetComponent(out Item item)) return;
        if (_itemFilter.Enabled && !_itemFilter.Value.Check(item)) return;
        _onEnterEvent.Raise(item);
    }
}