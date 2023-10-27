using System.Collections.Generic;
using System.Linq;
using Dynasty.Library;
using UnityEngine;

namespace Dynasty.Food {

public class StackModelProvider : ModelProvider {
    [SerializeField] float _toppingHeight;
    [SerializeField] Transform _bottom;
    [SerializeField] Transform _top;
    
    float _currentHeight;
    
    public override void SetBaseModel(Poolable poolable) {
        if (BaseModel != null) {
            BaseModel.Dispose();
        }

        BaseModel = poolable;
        
        var t = poolable.transform;
        t.SetParent(_bottom);
        t.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    public override void ReturnOriginalModel() {
        if (BaseModel != null) {
            BaseModel.Dispose();
        }

        BaseModel = null;
        _top.SetActive(true);
        _bottom.SetActive(true);

        while (Toppings.Count > 0) {
            Toppings.Pop().Dispose();
        }

        _top.localPosition -= _currentHeight * Vector3.up;
    }
    
    public override void AddToppingModel(Poolable model) {
        Toppings.Push(model);
        SetupToppingModel(model.gameObject);
    }

    void SetupToppingModel(GameObject model) {
        var pos = Vector3.up * Toppings.Count * _toppingHeight;

        var t = model.transform;
        t.SetParent(_bottom);
        t.SetLocalPositionAndRotation(pos, Quaternion.identity);

        _top.localPosition += _toppingHeight * Vector3.up;
        _currentHeight += _toppingHeight;
    }
}

}