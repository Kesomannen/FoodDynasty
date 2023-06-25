using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Food : MonoBehaviour, IPoolable<Food>, IInfoProvider {
    [SerializeField] Modifier _baseSellPrice = new(multiplicative: 1f);
    [SerializeField] GameObject _originalModel;
    [SerializeField] Transform _toppingParent;
    [SerializeField] FoodDataType[] _startingData;
    
    public Modifier SellPrice { get; set; }
    
    public double GetSellPrice() {
        return SellPrice.Apply(_baseSellPrice.Delta);
    }

    readonly Dictionary<Type, object> _data = new();
    readonly Stack<Poolable> _toppings = new();
    
    Poolable _baseModel;
    Rigidbody _rigidbody;

    public event Action<Food> OnDisposed;

    void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        Reset();
    }

    void Reset() {
        _data.Clear();
        foreach (var dataType in _startingData) {
            EnforceData(FoodDataUtil.GetDataType(dataType));
        }

        ReturnOriginalModel();
        while (_toppings.Count > 0) {
            _toppings.Pop().Dispose();
        }
        
        _rigidbody.velocity = Vector3.zero;
    }

    public void SetBaseModel(Poolable poolable) {
        if (_baseModel != null) {
            _baseModel.Dispose();
        }

        _baseModel = poolable;
        SetupModel(poolable.gameObject, ItemModelType.Base);
        
        _originalModel.SetActive(false);
    }

    public void ReturnOriginalModel() {
        if (_baseModel != null) {
            _baseModel.Dispose();
        }

        _baseModel = null;
        _originalModel.SetActive(true);
    }

    public void AddToppingModel(Poolable model) {
        _toppings.Push(model);
        SetupModel(model.gameObject, ItemModelType.Topping);
    }

    void SetupModel(GameObject model, ItemModelType type) {
        model.transform.SetParent(type == ItemModelType.Base ? transform : _toppingParent);
        model.transform.localPosition = Vector3.zero;
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

    public IEnumerable<(string Name, string Value)> GetInfo() {
        yield return ("Value", StringHelpers.FormatMoney(GetSellPrice()));
    }
    
    public void Dispose() {
        SellPrice = _baseSellPrice;
        Reset();
        
        OnDisposed?.Invoke(this);
    }
}

public enum ItemModelType {
    Base,
    Topping
}