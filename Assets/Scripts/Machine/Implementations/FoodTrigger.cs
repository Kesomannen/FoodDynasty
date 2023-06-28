using UnityEngine;

public class FoodTrigger : MonoBehaviour, IMachineComponent {
    [SerializeField] FilteredItemEvent _onTriggered;

    void OnTriggerEnter(Collider other) {
        if (!other.TryGetComponent(out Food food)) return;
        _onTriggered.Raise(food);
    }
    
    public Component Component => this;
}