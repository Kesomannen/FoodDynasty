using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dispenser : MonoBehaviour, IInfoProvider {
    [SerializeField] DataObject<float> _spawnSpeed;
    [SerializeField] Item _prefab;
    [SerializeField] Transform _spawnPoint;
    [Space]
    [SerializeField] LocalConditional<bool> _condition;
    [SerializeField] LocalEvent<Item> _dispenseEvent;

    void OnEnable() {
        StartCoroutine(DispenseLoop());
    }

    IEnumerator DispenseLoop() {
        while (enabled) {
            yield return CoroutineHelpers.Wait(1 / _spawnSpeed.Value);
            TryDispense();
        }
    }

    bool TryDispense() {
        if (!_condition.Check()) return false;
        
        var obj = Instantiate(_prefab, _spawnPoint.position, _spawnPoint.rotation);
        _dispenseEvent.Raise(obj);
        
        return true;
    }

    public IEnumerable<(string Name, string Value)> GetInfo() {
        yield return ("Speed", _spawnSpeed.Value.ToString("0"));
    }
}