using System.Collections.Generic;
using Dynasty.Library.Pooling;
using UnityEngine;

namespace Dynasty.Food {

public class DefaultModelProvider : ModelProvider {
    [SerializeField] GameObject _originalModel;
    [SerializeField] Transform _toppingParent;
    
    readonly Stack<Poolable> _toppings = new();
    
    Poolable _baseModel;
    
    public override void SetBaseModel(Poolable model) {
        if (_baseModel != null) {
            _baseModel.Dispose();
        }

        _baseModel = model;
        _originalModel.SetActive(false);
        
        model.transform.SetParent(transform);
        model.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    public override void ReturnOriginalModel() {
        if (_baseModel != null) {
            _baseModel.Dispose();
        }

        _baseModel = null;
        _originalModel.SetActive(true);
        
        while (_toppings.Count > 0) { 
            _toppings.Pop().Dispose();
        }
    }

    public override IEnumerable<Poolable> GetToppings() {
        return _toppings;
    }

    public override void AddToppingModel(Poolable model) {
        _toppings.Push(model);
        
        var rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        model.transform.SetParent(_toppingParent);
        model.transform.SetLocalPositionAndRotation(Vector3.zero, rotation);
    }
}

}