using System;
using Dynasty.Library.Extensions;
using UnityEngine;

[Serializable]
public struct FoodFieldFilter : IFilter<object> {
    public bool Enabled;
    public string FieldName;
    public FoodFieldFilterType FilterType;
    public Vector2Int IntRange;
    public bool BoolValue;

    public bool Check(object value) {
        return FilterType switch {
            FoodFieldFilterType.Int => IntRange.InRange((int) value),
            FoodFieldFilterType.Bool => (bool) value == BoolValue,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

public enum FoodFieldFilterType {
    Int,
    Bool
}