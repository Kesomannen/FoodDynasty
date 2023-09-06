using UnityEngine;

namespace Dynasty.Library.Extensions {

public static class EnumerableExtensions {
    public static T GetRandom<T>(this T[] array) {
        return array[Random.Range(0, array.Length)];
    }
}

}