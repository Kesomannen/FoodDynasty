using System.Collections.Generic;
using Dynasty.Food.Instance;
using Dynasty.Library.Classes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dynasty.Food.Modification {

[CreateAssetMenu(menuName = "Food/Modifier")]
public class FoodModifierGroup : ScriptableObject {
    [SerializeField] Modifier _sellPriceModifier = new(multiplicative: 1);
    [SerializeField] List<FoodModelModifier> _modelModifiers = new();
    [FormerlySerializedAs("_dataModifiers")]
    [SerializeField] List<FoodTraitModifier> _traitModifiers = new();
    
    public Modifier SellPriceModifier {
        get => _sellPriceModifier;
        set => _sellPriceModifier = value;
    }
    
    public List<FoodModelModifier> ModelModifiers => _modelModifiers;
    public List<FoodTraitModifier> TraitModifiers => _traitModifiers;

    public void Apply(FoodBehaviour food) {
        food.SellPrice += _sellPriceModifier;
        
        foreach (var modifier in _modelModifiers) {
            modifier.Apply(food);
        }

        foreach (var modifier in _traitModifiers) {
            modifier.Apply(food);
        }
    }
}

}