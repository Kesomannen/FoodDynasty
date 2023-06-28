using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class FoodDispenser : MonoBehaviour, IInfoProvider, IMachineComponent {
    [SerializeField] DataObject<float> _spawnSpeed;
    [SerializeField] CustomObjectPool<Food> _pool;
    [SerializeField] Transform _spawnPoint;
    [SerializeField] CheckEvent<bool> _condition;
    [SerializeField] Event<Food> _onDispensed;

    float _lastDispenseTime = float.MinValue;
    
    void OnEnable() {
        TickManager.AddListener(UpdateDispenseTimer);
    }
    
    void OnDisable() {
        TickManager.RemoveListener(UpdateDispenseTimer);
    }

    void UpdateDispenseTimer() {
        if (Time.time - _lastDispenseTime < 1 / _spawnSpeed.Value) return;
        _lastDispenseTime = Time.time;
        Dispense();
    }

    void Dispense() {
        if (!_condition.Check()) return;

        var food = _pool.Get();
        food.transform.SetPositionAndRotation(_spawnPoint.position, _spawnPoint.rotation);
        _onDispensed.Raise(food);
    }

    public IEnumerable<(string Name, string Value)> GetInfo() {
        yield return ("Speed", $"{_spawnSpeed.Value:0.#}");
    }
    
    public Component Component => this;
}