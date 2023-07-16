using System.Collections.Generic;
using Dynasty.Core.Data;
using Dynasty.Food.Instance;
using Dynasty.Library;
using Dynasty.Library.Entity;
using Dynasty.Library.Events;
using Dynasty.Library.Pooling;
using UnityEngine;

namespace Dynasty.Machine.Components {

public class FoodDispenser : MonoBehaviour, IInfoProvider, IMachineComponent {
    [SerializeField] DataObject<float> _spawnSpeed;
    [SerializeField] CustomObjectPool<FoodBehaviour> _pool;
    [SerializeField] Transform _spawnPoint;
    [SerializeField] Vector3 _randomSpawnOffset;
    [SerializeField] CheckEvent<bool> _condition;
    [SerializeField] Event<FoodBehaviour> _onDispensed;

    float _lastDispenseTime = float.MinValue;
    
    public CheckEvent<bool> Condition {
        get => _condition;
        set => _condition = value;
    }
    
    public Event<FoodBehaviour> DispenseEvent {
        get => _onDispensed;
        set => _onDispensed = value;
    }

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

        var position = _spawnPoint.position;
        if (_randomSpawnOffset != Vector3.zero) {
            position.x += Random.Range(-_randomSpawnOffset.x, _randomSpawnOffset.x);
            position.y += Random.Range(-_randomSpawnOffset.y, _randomSpawnOffset.y);
            position.z += Random.Range(-_randomSpawnOffset.z, _randomSpawnOffset.z);
        }
        food.transform.SetPositionAndRotation(position, _spawnPoint.rotation);
        _onDispensed.Raise(food);
    }

    public IEnumerable<EntityInfo> GetInfo() {
        var speed = _spawnSpeed == null ? "N/A" : $"{_spawnSpeed.Value:0.#}";
        yield return new EntityInfo("Speed", speed);
    }
    
    public Component Component => this;
}

}