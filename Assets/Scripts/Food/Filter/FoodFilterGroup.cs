using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Food/Filter")]
public class FoodFilterGroup : ScriptableObject, IFilter<Food> {
    [SerializeField] AndOr _condition;
    [SerializeField] bool _defaultValue = true;
    [SerializeField] FoodFilter[] _filters;

    public bool Check(Food food) {
        return Check(_filters, food);
    }

    bool Check(IReadOnlyCollection<IFilter<Food>> conditions, Food food) {
        if (conditions.Count == 0) return _defaultValue;
        
        return _condition switch {
            AndOr.And => conditions.All(condition => condition.Check(food)),
            AndOr.Or => conditions.Any(condition => condition.Check(food)),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    enum AndOr {
        And,
        Or
    }
}