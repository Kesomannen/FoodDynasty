using UnityEngine;

[CreateAssetMenu(menuName = "Food/Modifier")]
public class FoodModifierGroup : ScriptableObject {
    [SerializeField] Modifier _sellPriceModifier = new(multiplicative: 1);
    [SerializeField] FoodModelModifier[] _modelModifiers;
    [SerializeField] FoodDataModifier[] _dataModifiers;
    
    public Modifier SellPriceModifier => _sellPriceModifier;
    
    public void Apply(Food food) {
        food.SellPriceModifier += _sellPriceModifier;
        
        foreach (var modifier in _modelModifiers) {
            modifier.Apply(food);
        }

        foreach (var modifier in _dataModifiers) {
            modifier.Apply(food);
        }
    }
}