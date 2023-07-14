using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Dynasty.Library.Helpers {

public static class EnumHelpers {
    public static T GetRandom<T>() where T : Enum {
        var values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(Random.Range(0, values.Length));
    }
    
    public static IEnumerable<T> GetValues<T>() where T : Enum {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }

    public static Dictionary<TEnum, TValue> CreateDictionary<TEnum, TValue>(TEnum exclude, TValue value = default) where TEnum : Enum where TValue : struct {
        return CreateDictionary(exclude, _ => value);
    }
    
    public static Dictionary<TEnum, TValue> CreateDictionary<TEnum, TValue>(TValue value = default) where TEnum : Enum where TValue : struct {
        return CreateDictionary<TEnum, TValue>(_ => value);
    }
    
    public static Dictionary<TEnum, TValue> CreateDictionary<TEnum, TValue>(TEnum exclude, Func<TEnum, TValue> valueGetter) where TEnum : Enum {
        return GetValues<TEnum>()
            .Where(enumValue => !Equals(enumValue, exclude))
            .ToDictionary(enumValue => enumValue, valueGetter);
    }
    
    public static Dictionary<TEnum, TValue> CreateDictionary<TEnum, TValue>(Func<TEnum, TValue> valueGetter) where TEnum : Enum {
        return GetValues<TEnum>()
            .ToDictionary(enumValue => enumValue, valueGetter);
    }
}

}