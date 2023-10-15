using System.Collections.Generic;
using Dynasty.Food.Data;
using Dynasty.Library.Data;
using Dynasty.Food.Instance;
using Dynasty.Library;
using Dynasty.Library.Events;
using UnityEngine;

namespace Dynasty.Machines {

public class FoodDispenser : MonoBehaviour, IInfoProvider, IMachineComponent, IBoostableProperty {
    [SerializeField] FloatDataProperty _spawnSpeed;
    [SerializeField] FoodObjectPool _pool;
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

    void UpdateDispenseTimer(float delta) {
        if (Time.time - _lastDispenseTime < 1 / _spawnSpeed.Value) return;
        _lastDispenseTime = Time.time;
        TryDispense();
    }

    void TryDispense() {
        if (!_condition.Check()) return;
        if (!_pool.Spawn(out var food)) return;

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
    public FloatDataProperty BoostableProperty => _spawnSpeed;
}

}