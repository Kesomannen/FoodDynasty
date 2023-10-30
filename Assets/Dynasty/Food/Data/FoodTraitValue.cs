using System;
using Dynasty.Food;
using Dynasty.Library;
using UnityEngine;

namespace Dynasty.Food {

[Serializable]
public struct FoodTraitValue {
    [SerializeField] FoodTraitSelection _selection;
    [SerializeField] int _intValue;
    [SerializeField] float _floatValue;
    [SerializeField] bool _boolValue;
    [SerializeField] Modifier _modifierValue;

    public int Hash => _selection.Hash;
    public FoodTraitType Type => _selection.Type;
    
    object Value => Type switch {
        FoodTraitType.Int => _intValue,
        FoodTraitType.Float => _floatValue,
        FoodTraitType.Bool => _boolValue,
        FoodTraitType.Tag => null,
        FoodTraitType.Modifier => _modifierValue,
        _ => throw new ArgumentOutOfRangeException()
    };

    public void Apply(FoodBehaviour behaviour) {
        behaviour.SetTraitOrTag(Hash, Type, Value);
    }
}

}