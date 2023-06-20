using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public struct ItemModelModifier {
    [SerializeField] GameObject _modelPrefab;
    [SerializeField] ItemModelType _type;

    public void Apply(Item item) {
        var model = Object.Instantiate(_modelPrefab);
        switch (_type) {
            case ItemModelType.Base:
                item.SetBaseModel(model); break;
            case ItemModelType.Topping:
                item.AddToppingModel(model); break;
            default: throw new ArgumentOutOfRangeException();
        }
    }
}