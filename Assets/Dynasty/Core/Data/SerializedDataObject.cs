using GenericUnityObjects;
using UnityEngine;

namespace Dynasty.Core.Data {

/// <summary>
/// A <see cref="DataObject{T}"/> exposing its value as a serialized field.
/// </summary>
[CreateGenericAssetMenu]
public class SerializedDataObject<T> : DataObject<T> {
    [SerializeField] T _value;
    
    public override T Value {
        get => _value;
        set => _value = value;
    }
}

}