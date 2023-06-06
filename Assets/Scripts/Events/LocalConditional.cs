using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class LocalConditional<TIn, TOut> : MonoBehaviour {
    readonly List<Func<TIn>> _conditions = new();
    public  IReadOnlyList<Func<TIn>> Conditions => _conditions;

    public abstract TOut Check();
    
    public virtual void AddCondition(Func<TIn> condition) {
        _conditions.Add(condition);
    }
    
    public virtual void RemoveCondition(Func<TIn> condition) {
        _conditions.Remove(condition);
    }
}

public abstract class LocalConditional<T> : LocalConditional<T, T> { }