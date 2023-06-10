using UnityEngine;

public class ItemTrigger : MonoBehaviour {
    [SerializeField] Event<Item> _onTriggered;
    [SerializeField] Optional<ItemFilterGroup> _itemFilter;

    void OnTriggerEnter(Collider other) {
        if (!other.TryGetComponent(out Item item)) return;
        if (_itemFilter.Enabled && !_itemFilter.Value.Check(item)) return;
        _onTriggered.Raise(item);
    }
}