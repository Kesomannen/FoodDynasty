using System.Collections.Generic;
using Dynasty.Food;
using Dynasty.Library;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dynasty.Food {

[CreateAssetMenu(menuName = "Food/Modifier")]
public class FoodModifierGroup : ScriptableObject {
    [SerializeField] bool _random;
    [SerializeField] Modifier _sellPriceModifier = new(multiplicative: 1);
    [ShowIf("_random")] [AllowNesting]
    [SerializeField] Modifier _maxSellPriceModifier = new(multiplicative: 1);
    [SerializeField] List<FoodModelModifier> _modelModifiers = new();
    [FormerlySerializedAs("_dataModifiers")]
    [SerializeField] List<FoodTraitModifier> _traitModifiers = new();
    
    public bool IsRandom {
        get => _random;
        set => _random = value;
    }
    
    public Modifier SellPriceModifier {
        get => _sellPriceModifier;
        set => _sellPriceModifier = value;
    }
    
    public Modifier MaxSellPriceModifier {
        get => _maxSellPriceModifier;
        set => _maxSellPriceModifier = value;
    }
    
    public List<FoodModelModifier> ModelModifiers => _modelModifiers;
    public List<FoodTraitModifier> TraitModifiers => _traitModifiers;

    public void Apply(FoodBehaviour food) {
        food.SellPrice += _random ?
            Modifier.Lerp(_sellPriceModifier, _maxSellPriceModifier, Random.value) :
            _sellPriceModifier;
         
        foreach (var modifier in _modelModifiers) {
            modifier.Apply(food);
        }

        foreach (var modifier in _traitModifiers) {
            modifier.Apply(food);
        }
    }
}

}