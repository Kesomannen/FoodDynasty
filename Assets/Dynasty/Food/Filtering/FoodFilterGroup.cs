using System;
using System.Collections.Generic;
using System.Linq;
using Dynasty.Library;
using UnityEngine;

namespace Dynasty.Food {

[CreateAssetMenu(menuName = "Food/Filter")]
public class FoodFilterGroup : ScriptableObject {
    [SerializeField] AndOr _condition;
    [SerializeField] Optional<Vector2> _sellPriceRange;
    [SerializeField] FoodFilter[] _filters;

    public bool Check(FoodBehaviour food) {
        return Check(_filters, food);
    }

    bool Check(IReadOnlyCollection<FoodFilter> filters, FoodBehaviour food) {
        if (filters.Count == 0) return true;
        
        if (_sellPriceRange.Enabled) {
            var sellPrice = (float) food.SellPrice.Delta;
            if (_sellPriceRange.Value.InRange(sellPrice)) {
                return false;
            }
        }
        
        return _condition switch {
            AndOr.All => filters.All(condition => condition.Check(food)),
            AndOr.Any => filters.Any(condition => condition.Check(food)),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    enum AndOr {
        All,
        Any
    }
}

}