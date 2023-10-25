using UnityEngine;

namespace Dynasty.Library {

/// <summary>
/// Implementation of IDataProvider for editor referencing.
/// </summary>
public abstract class DataObject<T> : ScriptableObject, IDataProvider<T> {
    public abstract T Value { get; set; }
    public T Data => Value;
}

}