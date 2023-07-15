using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
    [SerializeField] FoodDataType[] _startingData;

    public Modifier SellPrice { get; set; }
    
    public bool IsSellable => _isSellable;
    public ModelProvider ModelProvider => _modelProvider;

    readonly Dictionary<Type, object> _data = new();
    
    bool _initialized;
    Rigidbody _rigidbody;

    public event Action<FoodBehaviour> OnDisposed;

    void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        ResetData();
        
        _initialized = true;
    }

    void ResetData() {
        SellPrice = _baseSellPrice;
        
        _data.Clear();
        foreach (var dataType in _startingData) {
            EnforceData(FoodDataUtil.GetDataType(dataType));
        }

        _modelProvider.ReturnOriginalModel();

        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }
    
    # region Data 
    
    public void SetData<T>(T data) where T : struct {
        _data[typeof(T)] = data;
    }
    
    public void SetData(Type type, object data) {
        _data[type] = data;
    }

    public object EnforceData(Type type) {
        var dataExists = RequireData(type, out var value);
        if (dataExists) return value;
        
        var newData = FormatterServices.GetUninitializedObject(type);
        SetData(type, newData);
        
        return newData;
    }

    public bool RequireData<T>(out T data) {
        var dataExists = RequireData(typeof(T), out var value);
        data = (T) value;
        return dataExists;
    }
    
    public bool RequireData(Type type, out object data) {
        if (_data.TryGetValue(type, out var value)) {
            data = value;
            return true;
        }

        data = default;
        return false;
    }
    
    # endregion

    public double GetSellPrice() {
        return _initialized ? SellPrice.Delta : _baseSellPrice.Delta;
    }
    
    public IEnumerable<EntityInfo> GetInfo() {
        yield return new EntityInfo("Value", StringHelpers.FormatMoney(GetSellPrice()));
    }
    
    public void Dispose() {
        ResetData();
        OnDisposed?.Invoke(this);
    }
}

}

public enum ItemModelType {
    Base,
    Topping
}