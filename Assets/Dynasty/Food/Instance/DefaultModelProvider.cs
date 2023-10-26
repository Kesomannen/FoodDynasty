using System.Collections.Generic;
using System.Linq;
using Dynasty.Library;
using UnityEngine;

namespace Dynasty.Food {

public class DefaultModelProvider : ModelProvider {
    [SerializeField] GameObject _originalModel;
    [SerializeField] Transform _toppingParent;
    
    public override void SetBaseModel(Poolable model) {
        if (BaseModel != null) {
            BaseModel.Dispose();
        }

        BaseModel = model;
        _originalModel.SetActive(false);
        
        model.transform.SetParent(transform);
        model.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    public override void ReturnOriginalModel() {
        if (BaseModel != null) {
            BaseModel.Dispose();
        }

        BaseModel = null;
        _originalModel.SetActive(true);
        
        while (Toppings.Count > 0) { 
            Toppings.Pop().Dispose();
        }
    }

    public override void AddToppingModel(Poolable model) {
        Toppings.Push(model);
        
        var rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        model.transform.SetParent(_toppingParent);
        model.transform.SetLocalPositionAndRotation(Vector3.zero, rotation);
    }
}

}