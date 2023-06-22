using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class Food : MonoBehaviour, IPoolable<Food>, IInfoProvider {
    [SerializeField] double _baseSellPrice;
    [SerializeField] GameObject _originalModel;
    [SerializeField] Transform _toppingParent;
    [SerializeField] FoodDataType[] _startingData;
    [SerializeField] Modifier _sellPriceModifier = new(multiplicative: 1f);

    public double BaseSellPrice => _baseSellPrice;

    public int Id { get; set; }

    public Modifier SellPriceModifier {
        get => _sellPriceModifier;
        set => _sellPriceModifier = value;
    }

    readonly Dictionary<Type, object> _data = new();
    readonly Stack<GameObject> _toppingModels = new();
    GameObject _baseModel;

    public event Action<Food> OnDisposed;

    void Awake() {
        Reset();
    }

    public double GetSellPrice() {
        return SellPriceModifier.Apply(_baseSellPrice);
    }

    void Reset() {
        _data.Clear();
        foreach (var dataType in _startingData) {
            EnforceData(FoodDataUtil.GetDataType(dataType));
        }

        SetBaseModel(_originalModel);
        while (_toppingModels.Count > 0) {
            Destroy(_toppingModels.Pop());
        }
    }

    public void SetBaseModel(GameObject model) {
        if (_baseModel != null) {
            if (_baseModel == _originalModel) _baseModel.SetActive(false);
            else Destroy(_baseModel);
        }

        _baseModel = model;
        SetupModel(model, ItemModelType.Base);
    }

    public void AddToppingModel(GameObject model) {
        _toppingModels.Push(model);
        SetupModel(model, ItemModelType.Topping);
    }

    void SetupModel(GameObject model, ItemModelType type) {
        model.transform.SetParent(type == ItemModelType.Base ? transform : _toppingParent);
        model.transform.localPosition = Vector3.zero;
    }

    # region Data 
    
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
    
    # endregion

    public IEnumerable<(string Name, string Value)> GetInfo() {
        yield return ("Value", StringHelpers.FormatMoney(GetSellPrice()));
    }
    
    public void Dispose() {
        SellPriceModifier = new Modifier(multiplicative: 1f);
        Reset();
        
        OnDisposed?.Invoke(this);
        Destroy(gameObject);
    }
}

public enum ItemModelType {
    Base,
    Topping
}