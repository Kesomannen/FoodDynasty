using System;
using System.Collections;
using Dynasty.Food;
using Dynasty.Library.Helpers;
using UnityEngine;

namespace Dynasty.Machines {

[RequireComponent(typeof(Conveyor))]
public class SplitConveyor : MonoBehaviour, IMachineComponent {
    [SerializeField] float _switchDelay;
    [SerializeField] Vector3[] _directions;
    
    int _index;
    Conveyor _conveyor;

    void Awake() {
        _conveyor = GetComponent<Conveyor>();
    }

    void OnEnable() {
        StartCoroutine(Routine());
        return;
        
        IEnumerator Routine() {
            while (enabled) {
                _conveyor.Direction = _directions[_index];
                _index = (_index + 1) % _directions.Length;
                yield return CoroutineHelpers.Wait(_switchDelay);
            }
        }
    }

    public Component Component => this;
}

}