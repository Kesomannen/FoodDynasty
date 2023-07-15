using System;
using System.Collections.Generic;
using System.Linq;
using Dynasty.Food.Data;
using Dynasty.Food.Instance;
using Dynasty.Library.Helpers;
using UnityEngine;

namespace Dynasty.Food.Filtering {

[Serializable]
public class FoodFilter {
    public ItemFilterType FilterType;
    public FoodDataType DataType;
    [Tooltip("If false, will create a new instance of the data type if it doesn't exist on the item. Otherwise, false is returned.")]
    public bool RequireData;
    
    public List<FieldFilter> FieldFilters;

    bool Has(FoodBehaviour food) {
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

    public bool Check(FoodBehaviour food) {
        return FilterType switch {
            ItemFilterType.Has => Has(food),
            ItemFilterType.DoesNotHave => !Has(food),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

}

public enum ItemFilterType {
    Has,
    DoesNotHave
}