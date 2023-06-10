using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ItemFilter : IFilter<Item> {
    public ItemFilterType FilterType;
    public ItemDataType DataType;
    [Tooltip("If false, will create a new instance of the data type if it doesn't exist on the item.")]
    public bool RequireData;
    
    public List<ItemFieldFilter> FieldFilters;

    bool Has(Item item) {
        var fieldFilters = FieldFilters;
        var type = ItemDataUtil.GetDataType(DataType);
        
        object data;
        if (RequireData) {
            var found = item.RequireData(type, out data);
            if (!found) return false;
        } else {
            data = item.EnforceData(type);
        }

        var enabledFieldFilters = fieldFilters.Where(fieldFilter => fieldFilter.Enabled).ToArray();
        
        if (enabledFieldFilters.Length == 0) return true;
        return enabledFieldFilters.All(fieldFilter => {
                var fieldValue = ReflectionHelpers.GetFieldValue(type, fieldFilter.FieldName, data);
                return fieldFilter.Check(fieldValue);
            });
    }

    public bool Check(Item item) {
        return FilterType switch {
            ItemFilterType.Has => Has(item),
            ItemFilterType.DoesNotHave => !Has(item),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

public enum ItemFilterType {
    Has,
    DoesNotHave
}