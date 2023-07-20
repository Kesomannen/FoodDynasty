using System;
using UnityEngine;

namespace Dynasty.Food.Data {

[Serializable]
public struct FoodTraitSelection {
    [SerializeField] string _value;
    [SerializeField] int _hash;
    [SerializeField] FoodTraitType _type;

    public int Hash => _hash;
    public FoodTraitType Type => _type;
}

}