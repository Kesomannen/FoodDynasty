using GenericUnityObjects;
using UnityEngine;

[CreateGenericAssetMenu]
public class DataObject<T> : ScriptableObject {
    [SerializeField] T _value;
    
    public T Value {
        get => _value;
        set => _value = value;
    }
}