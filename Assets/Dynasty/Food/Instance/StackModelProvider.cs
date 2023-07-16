using System.Collections.Generic;
using Dynasty.Library.Extensions;
using Dynasty.Library.Pooling;
using UnityEngine;

namespace Dynasty.Food.Instance {

public class StackModelProvider : ModelProvider {
    [SerializeField] Transform _bottom;
    [SerializeField] Transform _top;
    
    readonly Stack<Poolable> _toppings = new();
    
    Poolable _baseModel;
    
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
    }

    public override void AddToppingModel(Poolable model) {
        _toppings.Push(model);
        SetupToppingModel(model.gameObject);
    }

    void SetupToppingModel(GameObject model) {
        var colliders = model.GetComponentsInChildren<Collider>();
        var maxY = float.MinValue;
        var minY = float.MaxValue;
        
        foreach (var col in colliders) {
            var bounds = col.bounds;
            maxY = Mathf.Max(maxY, bounds.max.y);
            minY = Mathf.Min(minY, bounds.min.y);
        }
        
        var localPos = _toppings.Peek().transform.localPosition + Vector3.up * (maxY - minY);
        var modelTransform = model.transform;
        
        modelTransform.SetParent(_bottom);
        modelTransform.SetLocalPositionAndRotation(localPos, Quaternion.identity);
    }
}

}