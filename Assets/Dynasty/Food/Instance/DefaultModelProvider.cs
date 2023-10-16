using System.Collections.Generic;
using Dynasty.Library.Pooling;
using UnityEngine;

namespace Dynasty.Food {

public class DefaultModelProvider : ModelProvider {
    [SerializeField] GameObject _originalModel;
    [SerializeField] Transform _toppingParent;
    
    readonly Stack<Poolable> _toppings = new();
    
    Poolable _baseModel;
    
    public override void SetBaseModel(Poolable poolable) {
        if (_baseModel != null) {
            _baseModel.Dispose();
        }

        _baseModel = poolable;
        SetupModel(poolable.gameObject, ItemModelType.Base);
        
        _originalModel.SetActive(false);
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
        SetupModel(model.gameObject, ItemModelType.Topping);
    }

    void SetupModel(GameObject model, ItemModelType type) {
        model.transform.SetParent(type == ItemModelType.Base ? transform : _toppingParent);
        model.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
}

}