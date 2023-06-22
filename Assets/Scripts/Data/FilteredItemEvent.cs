using System;
using UnityEngine;

[Serializable]
public class FilteredItemEvent : IFilter<Food> {
    [SerializeField] Optional<Event<Food>> _event = new(null);
    [SerializeField] Optional<CheckEvent<bool>> _condition;
    [SerializeField] Optional<FoodFilterGroup> _filter;

    Action<Food> _subscriberEvent;
    int _subscribers;
    
    public void Raise(Food food) {
        if (!_event.Enabled || !Check(food)) return;
        _event.Value.Raise(food);
    }
    
    public void Subscribe(Action<Food> action) {
        AssertEventEnabled();
        
        _subscriberEvent += action;
        _subscribers++;
        UpdateSubscribers();
    }
    
    public void Unsubscribe(Action<Food> trigger) {
        AssertEventEnabled();
        
        _subscriberEvent -= trigger;
        _subscribers--;
        UpdateSubscribers();
    }

    void UpdateSubscribers() {
        if (!_event.Enabled) return;
        
        switch (_subscribers) {
            case 0: _event.Value.OnRaised -= OnEventRaised; break;
            case 1: _event.Value.OnRaised += OnEventRaised; break;
        }
    }

    void OnEventRaised(Food food) {
        if (Check(food)) {
            _subscriberEvent?.Invoke(food);
        }
    }
    
    public bool Check(Food food) {
        if (_condition.Enabled && !_condition.Value.Check()) return false;
        return !_filter.Enabled || _filter.Value.Check(food);
    }
    
    void AssertEventEnabled() {
        if (!_event.Enabled) {
            throw new Exception("Event is not enabled");
        }
    }
}