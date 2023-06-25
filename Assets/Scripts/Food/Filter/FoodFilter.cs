using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class FoodFilter : IFilter<Food> {
    public ItemFilterType FilterType;
    public FoodDataType DataType;
    [Tooltip("If false, will create a new instance of the data type if it doesn't exist on the item.")]
    public bool RequireData;
    
    public List<FoodFieldFilter> FieldFilters;

    bool Has(Food food) {
        var fieldFilters = FieldFilters;
        var type = FoodDataUtil.GetDataType(DataType);
        
        object data;
        if (RequireData) {
            var found = food.RequireData(type, out data);
            if (!found) return false;
        } else {
            data = food.EnforceData(type);
        }

        var enabledFieldFilters = fieldFilters.Where(fieldFilter => fieldFilter.Enabled).ToArray();
        
        if (enabledFieldFilters.Length == 0) return true;
        return enabledFieldFilters.All(fieldFilter => {
                var fieldValue = ReflectionHelpers.GetFieldValue(type, fieldFilter.FieldName, data);
                return fieldFilter.Check(fieldValue);
            });
    }

    public bool Check(Food food) {
        return FilterType switch {
            ItemFilterType.Has => Has(food),
            ItemFilterType.DoesNotHave => !Has(food),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

public enum ItemFilterType {
    Has,
    DoesNotHave
}