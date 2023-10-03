using System;
using Dynasty.Food.Filtering;
using Dynasty.Food.Instance;
using Dynasty.Library.Events;
using UnityEngine;

namespace Dynasty.Machines {

[Serializable]
public class FilteredFoodEvent {
    [SerializeField] Event<FoodBehaviour> _event;
    [SerializeField] CheckEvent<bool> _condition;
    [SerializeField] FoodFilterGroup _filter;

    Action<FoodBehaviour> _subscriberEvent;
    int _subscribers;

    public Event<FoodBehaviour> Event {
        get => _event;
        set => _event = value;
    }

    public CheckEvent<bool> Condition {
        get => _condition;
        set => _condition = value;
    }

    public FoodFilterGroup Filter {
        get => _filter;
        set => _filter = value;
    }

    public FilteredFoodEvent(Event<FoodBehaviour> @event, CheckEvent<bool> condition = null, FoodFilterGroup filter = null) {
        _event = @event;
        _condition = condition;
        _filter = filter;
    }
    
    public FilteredFoodEvent(Event<FoodBehaviour> @event, FoodFilterGroup filter) : this(@event, null, filter) { }
    
    public void Raise(FoodBehaviour food) {
        if (!Check(food)) return;
        _event.Raise(food);
    }

    public void Subscribe(Action<FoodBehaviour> action) {
        _subscriberEvent += action;
        _subscribers++;
        UpdateSubscribers();
    }

    public void Unsubscribe(Action<FoodBehaviour> trigger) {
        _subscriberEvent -= trigger;
        _subscribers--;
        UpdateSubscribers();
    }

    void UpdateSubscribers() {
        switch (_subscribers) {
            case 0:
                _event.OnRaised -= OnEventRaised;
                break;
            case 1:
                _event.OnRaised += OnEventRaised;
                break;
        }
    }

    void OnEventRaised(FoodBehaviour food) {
        if (Check(food)) {
            _subscriberEvent?.Invoke(food);
        }
    }

    public bool Check(FoodBehaviour food) {
        if (_condition != null && !_condition.Check()) return false;
        return _filter == null || _filter.Check(food);
    }
}

}