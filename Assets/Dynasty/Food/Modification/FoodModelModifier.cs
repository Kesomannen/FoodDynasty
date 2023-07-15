using System;
using Dynasty.Food.Instance;
using Dynasty.Library.Pooling;
using UnityEngine;

namespace Dynasty.Food.Modification {

[Serializable]
public struct FoodModelModifier {
    [SerializeField] CustomObjectPool<Poolable> _pool;
    [SerializeField] ItemModelType _type;

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