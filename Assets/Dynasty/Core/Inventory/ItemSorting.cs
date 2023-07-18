using System;
using System.Collections;
using System.Collections.Generic;

namespace Dynasty.Core.Inventory {

/// <summary>
/// Utility class for sorting items.
/// </summary>
public static class SortingUtil {
    public static IEnumerable<T> Sort<T>(IEnumerable<T> enumerable, Func<T, ItemData> dataGetter, ItemSortingMode mode, bool descending = false) {
        var list = new List<T>(enumerable);
        var comparison = GetComparison(mode, descending);
        list.Sort((itemA, itemB) => comparison(dataGetter(itemA), dataGetter(itemB)));
        return list;
    }

    public static IEnumerable<ItemData> Sorted(this IEnumerable<ItemData> enumerable, ItemSortingMode mode, bool descending = false) {
        var list = new List<ItemData>(enumerable);
        list.Sort(mode, descending);
        return list;
    }
    
    public static void Sort(this List<ItemData> list, ItemSortingMode mode, bool descending = false) {
        list.Sort(GetComparison(mode, descending));
    }

    public static Comparison<ItemData> GetComparison(ItemSortingMode mode, bool descending = false) {
        return mode switch {
            ItemSortingMode.Name => (itemA, itemB) => Compare(itemA, itemB, item => item.Name),
            ItemSortingMode.Price => (itemA, itemB) => Compare(itemA, itemB, item => item.Price),
            ItemSortingMode.Tier => (itemA, itemB) => Compare(itemA, itemB, item => (int)item.Tier),
            ItemSortingMode.Type => (itemA, itemB) => Compare(itemA, itemB, item => (int)item.Type),
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
        
        int Compare<T>(ItemData a, ItemData b, Func<ItemData, T> getter) where T : IComparable<T> {
            return descending ? getter(b).CompareTo(getter(a)) : getter(a).CompareTo(getter(b));
        }
    }
}

}

public enum ItemSortingMode {
    Name,
    Price,
    Tier,
    Type
}