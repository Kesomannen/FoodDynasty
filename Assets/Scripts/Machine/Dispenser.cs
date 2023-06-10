using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Dispenser : MonoBehaviour, IInfoProvider {
    [SerializeField] DataObject<float> _spawnSpeed;
    [SerializeField] Item _prefab;
    [SerializeField] Transform _spawnPoint;
    [Space]
    [SerializeField] CheckEvent<bool> _condition;
    [SerializeField] Event<Item> _onDispensed;

    void OnEnable() {
        StartCoroutine(DispenseLoop());
    }

    IEnumerator DispenseLoop() {
        while (enabled) {
            yield return CoroutineHelpers.Wait(1 / _spawnSpeed.Value);
            Dispense();
        }
    }

    void Dispense() {
        if (!_condition.Check()) return;

        var item = Instantiate(_prefab, _spawnPoint.position, _spawnPoint.rotation);
        _onDispensed.Raise(item);
    }

    public IEnumerable<(string Name, string Value)> GetInfo() {
        yield return ("Speed", _spawnSpeed.Value.ToString("0"));
    }
}