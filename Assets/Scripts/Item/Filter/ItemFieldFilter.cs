using System;
using UnityEngine;

[Serializable]
public struct ItemFieldFilter : IFilter<object> {
    public bool Enabled;
    public string FieldName;
    public ItemFieldFilterType FilterType;
    public Vector2Int IntRange;
    public bool BoolValue;

    public bool Check(object value) {
        return FilterType switch {
            ItemFieldFilterType.Int => IntRange.InRange((int) value),
            ItemFieldFilterType.Bool => (bool) value == BoolValue,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

public enum ItemFieldFilterType {
    Int,
    Bool
}