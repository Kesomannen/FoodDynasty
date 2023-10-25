using System.Collections.Generic;
using Dynasty.Library;
using UnityEngine;

namespace Dynasty.Machines {

[RequireComponent(typeof(Rigidbody))]
public class Conveyor : MonoBehaviour, IInfoProvider, IMachineComponent, IBoostableProperty {
    [SerializeField] Vector3 _direction = Vector3.forward;
    [SerializeField] FloatDataProperty _speed;
    
    Rigidbody _rigidbody;

    public Vector3 Direction {
        get => _direction;
        set => _direction = value;
    }

    void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void OnEnable() {
        TickManager.AddListener(OnTick);
    }
    
    void OnDisable() {
        TickManager.RemoveListener(OnTick);
    }

    void OnTick(float delta) {
        var newPosition = _rigidbody.position;
        var pos = newPosition;
        
        var localDirection = transform.rotation * _direction;
        newPosition += -localDirection * (_speed.Value * delta);
        _rigidbody.position = newPosition;
        
        _rigidbody.MovePosition(pos);
    }

    public IEnumerable<EntityInfo> GetInfo() {
        var speed = _speed == null ? "N/A" : $"{_speed.Value:0.#}";
        yield return new EntityInfo("Speed", speed);
    }

    void OnDrawGizmosSelected() {
        var t = transform;
        var position = t.position + Vector3.up * 0.5f;
        
        Gizmos.color = Color.red;
        var localDirection = t.rotation * _direction;
        Gizmos.DrawRay(position, localDirection);
    }

    public Component Component => this;
    public FloatDataProperty BoostableProperty => _speed;
}

}