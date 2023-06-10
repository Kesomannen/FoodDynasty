using GenericUnityObjects;
using UnityEngine;

[CreateGenericAssetMenu]
public class SerializedDataObject<T> : DataObject<T> {
    [SerializeField] T _value;
    
    public override T Value {
        get => _value;
        set => _value = value;
    }
}