using System;
using Dynasty.Food;
using Dynasty.Library;
using UnityEngine;

namespace Dynasty.Food {

[Serializable]
public class FoodFilter {
    [SerializeField] ItemFilterType _type;
    [SerializeField] FoodTraitSelection _trait;

    [SerializeField] bool _boolValue;
    [Tooltip("Inclusive")]
    [SerializeField] Vector2 _floatRange;
    [Tooltip("Inclusive")]
    [SerializeField] Vector2Int _intRange;

    bool Has(FoodBehaviour food) {
        return _trait.Type switch {
            FoodTraitType.Int => _intRange.InRange(food.GetTrait<int>(_trait.Hash)),
            FoodTraitType.Float => _floatRange.InRange(food.GetTrait<float>(_trait.Hash)),
            FoodTraitType.Bool => _boolValue == food.GetTrait<bool>(_trait.Hash),
            FoodTraitType.Tag => food.HasTag(_trait.Hash),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public bool Check(FoodBehaviour food) {
        return _type switch {
            ItemFilterType.Has => Has(food),
            ItemFilterType.DoesNotHave => !Has(food),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    public enum ItemFilterType {
        Has,
        DoesNotHave
    }
}

}