using System.Linq;
using UnityEngine;

public class Condition : CheckEvent<bool> {
    [SerializeField] bool _defaultState;

    public override bool Check() {
        return Conditions.Count == 0 ? _defaultState : Conditions.All(condition => condition.Invoke());
    }
}