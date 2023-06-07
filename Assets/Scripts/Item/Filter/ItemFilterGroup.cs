using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ItemFilterGroup : IFilter<Item> {
    [SerializeField] AndOr _condition;
    [SerializeField] bool _defaultValue = true;
    [SerializeField] ItemFilter[] _filters;

    public bool Check(Item item) {
        return Check(_filters, item);
    }

    bool Check(IReadOnlyCollection<IFilter<Item>> conditions, Item item) {
        if (conditions.Count == 0) return _defaultValue;
        
        return _condition switch {
            AndOr.And => conditions.All(condition => condition.Check(item)),
            AndOr.Or => conditions.Any(condition => condition.Check(item)),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    enum AndOr {
        And,
        Or
    }
}