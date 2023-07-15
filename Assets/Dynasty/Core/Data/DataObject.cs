using UnityEngine;

namespace Dynasty.Core.Data {

public abstract class DataObject<T> : ScriptableObject, IDataProvider<T> {
    public abstract T Value { get; set; }
    public T Data => Value;
}

}