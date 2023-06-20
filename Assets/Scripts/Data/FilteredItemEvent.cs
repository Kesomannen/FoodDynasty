using System;
using UnityEngine;

[Serializable]
public class FilteredItemEvent : IFilter<Item> {
    [SerializeField] Optional<Event<Item>> _event = new(null);
    [SerializeField] Optional<CheckEvent<bool>> _condition;
    [SerializeField] Optional<ItemFilterGroup> _filter;

    Action<Item> _subscriberEvent;
    int _subscribers;
    
    public void Raise(Item item) {
        if (!_event.Enabled || !Check(item)) return;
        _event.Value.Raise(item);
    }
    
    public void Subscribe(Action<Item> action) {
        AssertEventEnabled();
        
        _subscriberEvent += action;
        _subscribers++;
        UpdateSubscribers();
    }
    
    public void Unsubscribe(Action<Item> trigger) {
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

    void OnEventRaised(Item item) {
        if (Check(item)) {
            _subscriberEvent?.Invoke(item);
        }
    }
    
    public bool Check(Item item) {
        if (_condition.Enabled && !_condition.Value.Check()) return false;
        return !_filter.Enabled || _filter.Value.Check(item);
    }
    
    void AssertEventEnabled() {
        if (!_event.Enabled) {
            throw new Exception("Event is not enabled");
        }
    }
}