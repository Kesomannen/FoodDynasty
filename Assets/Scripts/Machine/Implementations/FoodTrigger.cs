using UnityEngine;

public class FoodTrigger : MonoBehaviour, IMachineComponent {
    [SerializeField] FilteredFoodEvent _onTriggered;

    public FilteredFoodEvent TriggerEvent {
        get => _onTriggered;
        set => _onTriggered = value;
    }

    void OnTriggerEnter(Collider other) {
        if (!other.TryGetComponent(out Food food)) return;
        _onTriggered.Raise(food);
    }
    
    public Component Component => this;
}