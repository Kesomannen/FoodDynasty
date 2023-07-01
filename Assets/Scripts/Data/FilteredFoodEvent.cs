using System;
using UnityEngine;

[Serializable]
public class FilteredFoodEvent : IFilter<Food> {
    [SerializeField] Event<Food> _event;
    [SerializeField] CheckEvent<bool> _condition;
    [SerializeField] FoodFilterGroup _filter;

    Action<Food> _subscriberEvent;
    int _subscribers;

    public Event<Food> Event {
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
    
    public FilteredFoodEvent(Event<Food> @event) {
        _event = @event;
    }
    
    public void Raise(Food food) {
        if (!Check(food)) return;
        _event.Raise(food);
    }
    
    public void Subscribe(Action<Food> action) {
        _subscriberEvent += action;
        _subscribers++;
        UpdateSubscribers();
    }
    
    public void Unsubscribe(Action<Food> trigger) {
        _subscriberEvent -= trigger;
        _subscribers--;
        UpdateSubscribers();
    }

    void UpdateSubscribers() {
        switch (_subscribers) {
            case 0: _event.OnRaised -= OnEventRaised; break;
            case 1: _event.OnRaised += OnEventRaised; break;
        }
    }

    void OnEventRaised(Food food) {
        if (Check(food)) {
            _subscriberEvent?.Invoke(food);
        }
    }
    
    public bool Check(Food food) {
        if (_condition != null && !_condition.Check()) return false;
        return _filter == null || _filter.Check(food);
    }
}