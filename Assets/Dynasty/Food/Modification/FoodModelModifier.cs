using System;
using Dynasty.Food;
using Dynasty.Library;
using UnityEngine;

namespace Dynasty.Food {

[Serializable]
public struct FoodModelModifier {
    [SerializeField] CustomObjectPool<Poolable> _pool;
    [SerializeField] ItemModelType _type;

    public FoodModelModifier(CustomObjectPool<Poolable> pool, ItemModelType type) {
        _pool = pool;
        _type = type;
    }
    
    public void Apply(FoodBehaviour food) {
        switch (_type) {
            case ItemModelType.Base:
                food.ModelProvider.SetBaseModel(_pool.Get()); break;
            case ItemModelType.Topping:
                food.ModelProvider.AddToppingModel(_pool.Get()); break;
            default: throw new ArgumentOutOfRangeException();
        }
    }
}

}