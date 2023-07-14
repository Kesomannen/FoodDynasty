using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dynasty.Library.Events {

public abstract class CheckEvent<T> : MonoBehaviour {
    readonly List<Func<T>> _conditions = new();
    public IReadOnlyList<Func<T>> Conditions => _conditions;

    public abstract T Check();
    
    public virtual void AddCondition(Func<T> condition) {
        _conditions.Add(condition);
    }
    
    public virtual void RemoveCondition(Func<T> condition) {
        _conditions.Remove(condition);
    }
}

}