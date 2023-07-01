using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Conveyor : MonoBehaviour, IInfoProvider, IMachineComponent {
    [SerializeField] Vector3 _direction = Vector3.forward;
    [SerializeField] DataObject<float> _speed;
    
    Rigidbody _rigidbody;
    
    public DataObject<float> Speed {
        get => _speed;
        set => _speed = value;
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

    void OnTick() {
        var newPosition = _rigidbody.position;
        var pos = newPosition;
        
        var localDirection = transform.rotation * _direction;
        newPosition += -localDirection * (_speed.Value * Time.fixedDeltaTime);
        _rigidbody.position = newPosition;
        
        _rigidbody.MovePosition(pos);
    }

    public IEnumerable<(string Name, string Value)> GetInfo() {
        yield return ("Speed", $"{_speed.Value:0.#}");
    }

    void OnDrawGizmosSelected() {
        var t = transform;
        var position = t.position + Vector3.up * 0.5f;
        
        Gizmos.color = Color.red;
        var localDirection = t.rotation * _direction;
        Gizmos.DrawRay(position, localDirection);
    }

    public Component Component => this;
}