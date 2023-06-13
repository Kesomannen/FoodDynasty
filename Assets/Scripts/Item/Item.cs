using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class Item : MonoBehaviour, IPoolable<Item>, IInfoProvider {
    [SerializeField] double _baseSellPrice;
    [SerializeField] GameObject _originalModel;
    [SerializeField] Vector3 _toppingOffset;
    [SerializeField] ItemDataType[] _startingData;
    [SerializeField] Modifier _sellPriceModifier = new(multiplicative: 1f);

    public double BaseSellPrice => _baseSellPrice;

    public Modifier SellPriceModifier {
        get => _sellPriceModifier;
        set {
            Debug.Log($"Setting sell price modifier to {value}", this);
            _sellPriceModifier = value;
        }
    }

    readonly Dictionary<Type, object> _data = new();
    readonly Stack<GameObject> _models = new();
    GameObject _baseModel;

    public event Action<Item> OnDisposed;

    void Awake() {
        Reset();
    }

    public double GetSellPrice() {
        return SellPriceModifier.Apply(_baseSellPrice);
    }

    void Reset() {
        _data.Clear();
        foreach (var dataType in _startingData) {
            EnforceData(ItemDataUtil.GetDataType(dataType));
        }

        SetBaseModel(_originalModel);
        while (_models.Count > 0) {
            Destroy(_models.Pop());
        }
    }

    public void SetBaseModel(GameObject model) {
        if (_baseModel == model) return;
        
        if (_baseModel) {
            Destroy(_baseModel);
        }

        _baseModel = model;
        SetupModel(model, ItemModelPivot.Base);
    }

    public void AddTopping(GameObject model) {
        _models.Push(model);
        SetupModel(model, ItemModelPivot.Topping);
    }

    void SetupModel(GameObject model, ItemModelPivot pivot) {
        model.transform.SetParent(transform);
        model.transform.localPosition = pivot == ItemModelPivot.Base ? Vector3.zero : _toppingOffset;
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

public enum ItemModelPivot {
    Base,
    Topping
}