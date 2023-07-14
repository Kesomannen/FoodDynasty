using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Dynasty.Library.Helpers {

public static class ReflectionHelpers {
    static readonly Dictionary<Type, FieldInfo[]> _cachedFields = new();

    public static FieldInfo[] GetFields<T>() {
        return GetFields(typeof(T));
    }
    
    public static FieldInfo[] GetFields(Type type) {
        if (_cachedFields.TryGetValue(type, out var fields)) {
            return fields;
        }

        fields = type.GetFields();
        _cachedFields[type] = fields;
        return fields;
    }
    
    public static FieldInfo GetField<T>(string name) {
        return GetFields<T>().First(field => field.Name == name);
    }
    
    public static FieldInfo GetField(Type type, string name) {
        return GetFields(type).First(field => field.Name == name);
    }
    
    public static bool TryGetField(Type type, string name, out FieldInfo field) {
        field = GetFields(type).FirstOrDefault(field => field.Name == name);
        return field != null;
    }
    
    public static object GetFieldValue<T>(string name, T instance) {
        return GetField<T>(name).GetValue(instance);
    }
    
    public static object GetFieldValue(Type type, string name, object instance) {
        return GetField(type, name).GetValue(instance);
    }
}

}