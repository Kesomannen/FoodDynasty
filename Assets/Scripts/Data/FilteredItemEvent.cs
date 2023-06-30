using System;
using UnityEngine;

[Serializable]
public class FilteredItemEvent : IFilter<Food> {
    [SerializeField] Event<Food> _event;
    [SerializeField] CheckEvent<bool> _condition;
    [SerializeField] FoodFilterGroup _filter;

    Action<Food> _subscriberEvent;
    int _subscribers;
    
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