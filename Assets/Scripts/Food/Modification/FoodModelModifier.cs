using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public struct FoodModelModifier {
    [SerializeField] GameObject _modelPrefab;
    [SerializeField] ItemModelType _type;

    public void Apply(Food food) {
        var model = Object.Instantiate(_modelPrefab);
        switch (_type) {
            case ItemModelType.Base:
                food.SetBaseModel(model); break;
            case ItemModelType.Topping:
                food.AddToppingModel(model); break;
            default: throw new ArgumentOutOfRangeException();
        }
    }
}