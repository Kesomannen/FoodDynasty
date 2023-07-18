using System;
using Dynasty.Food.Instance;
using UnityEngine;

namespace Dynasty.Food.Data {

[Serializable]
public struct FoodTraitValue {
    [SerializeField] FoodTraitSelection _selection;
    [SerializeField] int _intValue;
    [SerializeField] float _floatValue;
    [SerializeField] bool _boolValue;

    public FoodTraitType Get(out int hash, out object value) {
        _selection.GetEntry(out var entry);
        hash = entry.Hash;

        value = entry.Type switch {
            FoodTraitType.Tag => null,
            FoodTraitType.Int => _intValue,
            FoodTraitType.Float => _floatValue,
            FoodTraitType.Bool => _boolValue,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        return entry.Type;
    }

    public void Apply(FoodBehaviour behaviour) {
        Get(out var hash, out var value);
        behaviour.SetTrait(hash, value);
    }
}

}