using System.Collections.Generic;
using Dynasty.Library;
using UnityEngine;

namespace Dynasty.Food {

public class StackModelProvider : ModelProvider {
    [SerializeField] float _toppingHeight;
    [SerializeField] Transform _bottom;
    [SerializeField] Transform _top;
    
    readonly Stack<Poolable> _toppings = new();
    
    Poolable _baseModel;
    Vector3 _originalTopPosition;
    
    void Awake() {
        _originalTopPosition = _top.localPosition;
    }
    
    public override void SetBaseModel(Poolable poolable) {
        if (_baseModel != null) {
            _baseModel.Dispose();
        }

        _baseModel = poolable;
        
        var poolableTransform = poolable.transform;
        poolableTransform.SetParent(_bottom);
        poolableTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        
        _top.SetActive(false);
        _bottom.SetActive(false);
    }

    public override void ReturnOriginalModel() {
        if (_baseModel != null) {
            _baseModel.Dispose();
        }

        _baseModel = null;
        _top.SetActive(true);
        _bottom.SetActive(true);
        
        while (_toppings.Count > 0) { 
            _toppings.Pop().Dispose();
        }
        _top.localPosition = _originalTopPosition;
    }

    public override IEnumerable<Poolable> GetToppings() {
        return _toppings;
    }

    public override void AddToppingModel(Poolable model) {
        _toppings.Push(model);
        SetupToppingModel(model.gameObject);
    }

    void SetupToppingModel(GameObject model) {
        var pos = Vector3.up * _toppings.Count * _toppingHeight;

        var modelTransform = model.transform;
        modelTransform.SetParent(_bottom);
        modelTransform.SetLocalPositionAndRotation(pos, Quaternion.identity);

        _top.localPosition += _toppingHeight * Vector3.up;
    }
}

}