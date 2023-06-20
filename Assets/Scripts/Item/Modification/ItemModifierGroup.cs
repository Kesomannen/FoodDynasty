using UnityEngine;

[CreateAssetMenu(menuName = "Item/Modifier")]
public class ItemModifierGroup : ScriptableObject {
    [SerializeField] Modifier _sellPriceModifier = new(multiplicative: 1);
    [SerializeField] ItemModelModifier[] _modelModifiers;
    [SerializeField] ItemDataModifier[] _dataModifiers;
    
    public Modifier SellPriceModifier => _sellPriceModifier;
    
    public void Apply(Item item) {
        item.SellPriceModifier += _sellPriceModifier;
        
        foreach (var modifier in _modelModifiers) {
            modifier.Apply(item);
        }

        foreach (var modifier in _dataModifiers) {
            modifier.Apply(item);
        }
    }
}