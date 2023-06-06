using UnityEngine;

public class ItemTrigger : MonoBehaviour {
    [SerializeField] LocalEvent<Item> _onEnterEvent;
    
    void OnTriggerEnter(Collider other) {
        if (!other.TryGetComponent(out Item item)) return;
        _onEnterEvent.Raise(item);
    }
}