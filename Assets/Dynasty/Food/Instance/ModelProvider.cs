using System.Collections.Generic;
using System.Linq;
using Dynasty.Library;
using UnityEngine;

namespace Dynasty.Food {

public abstract class ModelProvider : MonoBehaviour {
    public abstract void SetBaseModel(Poolable model);
    public abstract void AddToppingModel(Poolable model);
    public abstract void ReturnOriginalModel();
    
    protected readonly Stack<Poolable> Toppings = new();
    
    protected Poolable BaseModel;
    
    public IEnumerable<CustomObjectPool<Poolable>> GetModelPools() {
        if (BaseModel != null) {
            yield return BaseModel.Pool;
        }
        foreach (var topping in Toppings) {
            yield return topping.Pool;
        }
    }
    
    public void AddToppingModels(IEnumerable<Poolable> models) {
        foreach (var model in models) {
            AddToppingModel(model);
        }
    }
    
    public void AddToppingModels(IEnumerable<CustomObjectPool<Poolable>> pools) {
        AddToppingModels(pools.Select(p => p.Get()));
    }
}

}