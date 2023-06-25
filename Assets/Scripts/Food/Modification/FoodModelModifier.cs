using System;
using NaughtyAttributes;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public struct FoodModelModifier {
    [SerializeField] CustomObjectPool<Poolable> _pool;
    [SerializeField] ItemModelType _type;

    public void Apply(Food food) {
        switch (_type) {
            case ItemModelType.Base:
                food.SetBaseModel(_pool.Get()); break;
            case ItemModelType.Topping:
                food.AddToppingModel(_pool.Get()); break;
            default: throw new ArgumentOutOfRangeException();
        }
    }
}