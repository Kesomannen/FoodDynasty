using System;
using Dynasty.Food.Data;
using Dynasty.Food.Instance;
using Dynasty.Library.Extensions;
using UnityEngine;

namespace Dynasty.Food.Filtering {

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
        _trait.GetEntry(out var entry);

        return entry.Type switch {
            FoodTraitType.Int => _intRange.InRange(food.GetTrait<int>(entry.Hash)),
            FoodTraitType.Float => _floatRange.InRange(food.GetTrait<float>(entry.Hash)),
            FoodTraitType.Bool => _boolValue == food.GetTrait<bool>(entry.Hash),
            FoodTraitType.Tag => food.HasTag(entry.Hash),
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