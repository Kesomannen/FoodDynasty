using UnityEngine;

namespace Dynasty.Library.Data {

/// <summary>
/// Implementation of IDataProvider for editor referencing.
/// </summary>
public abstract class DataObject<T> : ScriptableObject, IDataProvider<T> {
    public abstract T Value { get; set; }
    public T Data => Value;
}

}