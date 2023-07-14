using System.Linq;
using Dynasty.Library.Events;
using UnityEngine;

namespace Dynasty.Library.Events {

public class Condition : CheckEvent<bool> {
    [SerializeField] bool _defaultState;

    public override bool Check() {
        return Conditions.Count == 0 ? _defaultState : Conditions.All(condition => condition.Invoke());
    }
}

}