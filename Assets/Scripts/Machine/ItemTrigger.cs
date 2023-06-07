using UnityEngine;

public class ItemTrigger : MonoBehaviour {
    [SerializeField] ItemFilterGroup _itemFilter;
    [SerializeField] LocalEvent<Item> _onEnterEvent;

    void OnTriggerEnter(Collider other) {
        if (!other.TryGetComponent(out Item item)) return;
        if (!_itemFilter.Check(item)) return;
        _onEnterEvent.Raise(item);
    }
}