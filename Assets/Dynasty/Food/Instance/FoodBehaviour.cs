using System;
using System.Collections.Generic;
using Dynasty.Food.Data;
using Dynasty.Library.Entity;
using Dynasty.Library.Classes;
using Dynasty.Library.Helpers;
using Dynasty.Library.Pooling;
using UnityEngine;

namespace Dynasty.Food.Instance {

[RequireComponent(typeof(Rigidbody))]
public class FoodBehaviour : MonoBehaviour, IPoolable<FoodBehaviour>, IInfoProvider {
    [SerializeField] ModelProvider _modelProvider;
    [SerializeField] Modifier _baseSellPrice = new(multiplicative: 1f);
    [SerializeField] bool _isSellable = true;
    [SerializeField] FoodTraitValue[] _startingTraits;

    public Modifier SellPrice { get; set; }
    
    public bool IsSellable => _isSellable;
    public ModelProvider ModelProvider => _modelProvider;

    readonly Dictionary<int, object> _traits = new();
    readonly List<int> _tags = new();

    bool _initialized;
    Rigidbody _rigidbody;

    public event Action<FoodBehaviour> OnDisposed;

    void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Start() {
        ResetState();
        _initialized = true;
    }

    public double GetSellPrice() {
        return _initialized ? SellPrice.Delta : _baseSellPrice.Delta;
    }

    void ResetState() {
        SellPrice = _baseSellPrice;

        _modelProvider.ReturnOriginalModel();

        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        
        _traits.Clear();
        _tags.Clear();
        
        foreach (var starting in _startingTraits) {
            if (starting.Get(out var hash, out var value) == FoodTraitType.Tag) AddTag(hash);
            else SetTrait(hash, value);
        }
    }
    
    public void AddTag(int hash) {
        if (!_tags.Contains(hash)) {
            _tags.Add(hash);
        }
    }
    
    public void SetTrait<T>(int hash, T value) {
        _traits[hash] = value;
    }

    public void SetTrait(int hash, FoodTraitType type, object value) {
        if (type == FoodTraitType.Tag) AddTag(hash);
        else SetTrait(hash, value);
    }
    
    public bool HasTag(int hash) {
        return _tags.Contains(hash);
    }
    
    public T GetTrait<T>(int hash) {
        TryGetTrait(hash, out T value);
        return value;
    }
    
    public bool TryGetTrait<T>(int hash, out T value) {
        var success = _traits.TryGetValue(hash, out var objValue);
        value = (T) objValue;
        return success;
    }
    
    public IEnumerable<EntityInfo> GetInfo() {
        yield return new EntityInfo("Value", StringHelpers.FormatMoney(GetSellPrice()));
    }
    
    public void Dispose() {
        ResetState();
        OnDisposed?.Invoke(this);
    }
}

}

public enum ItemModelType {
    Base,
    Topping
}