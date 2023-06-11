using System;
using System.Collections.Generic;
using UnityEngine;

public static class UIHelpers {
    public static void SortSiblingIndices<T>(this IEnumerable<Container<T>> items, Comparison<T> comparison) {
        items.SortSiblingIndices(container => container.Content, comparison);
    }
    
    public static void SortSiblingIndices<TItem, TData>(this IEnumerable<TItem> items, Func<TItem, TData> dataGetter, Comparison<TData> comparison)
    where TItem : Component {
        var sorted = new List<TItem>(items);
        sorted.Sort((itemA, itemB) => comparison(dataGetter(itemA), dataGetter(itemB)));

        for (var i = 0; i < sorted.Count; i++) {
            sorted[i].transform.SetSiblingIndex(i);
        }
    }
} 