using System;
using UnityEngine;

namespace Dynasty.Food.Data {

[Serializable]
public struct FoodTraitSelection {
    [SerializeField] string _value;
    [SerializeField] int _hash;

    public bool GetEntry(out FoodTraitDatabase.Entry entry) {
        return FoodTraitDatabase.Singleton.TryGetEntry(_hash, out entry);
    }
}

}