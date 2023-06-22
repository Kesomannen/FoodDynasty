using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class ItemDispenser : MonoBehaviour, IInfoProvider {
    [Expandable]
    [SerializeField] DataObject<float> _spawnSpeed;
    [SerializeField] Food _prefab;
    [SerializeField] Transform _spawnPoint;
    [Space]
    [SerializeField] CheckEvent<bool> _condition;
    [SerializeField] Event<Food> _onDispensed;

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
        yield return ("Speed", $"{_spawnSpeed.Value:0.#}");
    }
}