using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Dynasty.Library.Extensions {

public static class EnumerableExtensions {
    public static T GetRandom<T>(this T[] array) {
        return array[Random.Range(0, array.Length)];
    }
    
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) {
        foreach (var item in enumerable) {
            action(item);
        }
    }
}

}