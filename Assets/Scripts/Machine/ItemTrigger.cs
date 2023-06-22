using UnityEngine;

public class ItemTrigger : MonoBehaviour {
    [SerializeField] FilteredItemEvent _onTriggered;

    void OnTriggerEnter(Collider other) {
        if (!other.TryGetComponent(out Food item)) return;
        _onTriggered.Raise(item);
    }
}