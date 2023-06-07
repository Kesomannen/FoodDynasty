using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class Item : MonoBehaviour, IDisposable {
    [SerializeField] float _baseSellPrice;
    [SerializeField] ItemDataType[] _startingData;

    public float BaseSellPrice => _baseSellPrice;
    public Modifier SellPriceModifier { get; set; } = new(multiplicative: 1f);
    
    readonly Dictionary<Type, object> _data = new();
    
    public event Action<Item> OnDisposed;

    void Awake() {
        SetupData();
    }

    public void SetData<T>(T data) {
        _data[typeof(T)] = data;
    }
    
    public void SetData(Type type, object data) {
        _data[type] = data;
    }

    public object EnforceData(Type type) {
        var found = RequireData(type, out var value);
        if (found) {
            return (ValueType) value;
        }

        var obj = FormatterServices.GetUninitializedObject(type);
        SetData(type, obj);
        return obj;
    }

    public bool RequireData<T>(out T data) {
        var found = RequireData(typeof(T), out var value);
        data = (T) value;
        return found;
    }
    
    public bool RequireData(Type type, out object data) {
        if (_data.TryGetValue(type, out var value)) {
            data = value;
            return true;
        }

        data = default;
        return false;
    }

    public void Dispose() {
        _data.Clear();
        SellPriceModifier = new Modifier(multiplicative: 1f);
        OnDisposed?.Invoke(this);
    }

    void SetupData() {
        _data.Clear();
        foreach (var dataType in _startingData) {
            EnforceData(ItemDataUtil.GetDataType(dataType));
        }
    }
}