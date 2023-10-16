using System.Collections.Generic;
using Dynasty.Library.Extensions;
using Dynasty.Library.Pooling;
using UnityEngine;

namespace Dynasty.Food {

public class StackModelProvider : ModelProvider {
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
        SetupToppingModel(model.gameObject);
        _toppings.Push(model);
    }

    void SetupToppingModel(GameObject model) {
        var renderers = model.GetComponentsInChildren<Renderer>();
        var maxY = float.MinValue;
        var minY = float.MaxValue;
        
        foreach (var renderer in renderers) {
            var bounds = renderer.bounds;
            maxY = Mathf.Max(maxY, bounds.max.y);
            minY = Mathf.Min(minY, bounds.min.y);
        }

        var height = Vector3.up * (maxY - minY) * 1.2f;
        var previous = _toppings.Count > 0 ? _toppings.Peek().transform.localPosition : Vector3.zero;

        var modelTransform = model.transform;
        modelTransform.SetParent(_bottom);
        modelTransform.SetLocalPositionAndRotation(previous + height, Quaternion.identity);
        
        _top.localPosition += height;
    }
}

}