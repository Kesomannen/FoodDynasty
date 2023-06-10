using UnityEngine;

public abstract class DataObject<T> : ScriptableObject {
    public abstract T Value { get; set; }
}