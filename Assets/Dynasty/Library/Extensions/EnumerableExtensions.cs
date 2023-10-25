using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Dynasty.Library {

public static class EnumerableExtensions {
    public static T GetRandom<T>(this T[] array) {
        return array[Random.Range(0, array.Length)];
    }

    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) {
        foreach (var item in enumerable) {
            action(item);
        }
    }
    
    public static int IndexOf<T>(this IEnumerable<T> enumerable, T item) {
        var index = 0;
        foreach (var i in enumerable) {
            if (Equals(i, item)) return index;
            index++;
        }

        return -1;
    }
}

}