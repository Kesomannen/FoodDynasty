using System.Collections.Generic;
using Dynasty.Library.Pooling;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dynasty.UI.Menu {

public class MenuBackground : MonoBehaviour {
    [SerializeField] CustomObjectPool<PoolableComponent<Rigidbody>> _breadPool;
    [SerializeField] Vector3 _spawnArea;
    [SerializeField] float _despawnY;
    [SerializeField] float _spawnInterval;
    [SerializeField] float _gravity;

    readonly List<PoolableComponent<Rigidbody>> _bread = new();
    
    float _originalGravity;
    float _timer;

    void Awake() {
        _originalGravity = Physics.gravity.y;
        Physics.gravity = new Vector3(0, _gravity, 0);
    }

    void Update() {
        for (var i = 0; i < _bread.Count; i++) {
            var bread = _bread[i];
            if (bread.transform.position.y > _despawnY) continue;
            
            _bread.RemoveAt(i);
            bread.Dispose();
            i--;
        }
        
        _timer += Time.deltaTime;
        if (_timer < _spawnInterval) return;
        
        _timer = 0;
        var pos = transform.position + new Vector3(
            Random.Range(-_spawnArea.x, _spawnArea.x),
            Random.Range(-_spawnArea.y, _spawnArea.y),
            Random.Range(-_spawnArea.z, _spawnArea.z)
        );
        
        var newBread = _breadPool.Get();
        var rb = newBread.Component;
        
        newBread.transform.position = pos;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero * Random.Range(-1f, 1f);
        
        _bread.Add(newBread);
    }

    void OnDestroy() {
        _breadPool.Clear();
        Physics.gravity = new Vector3(0, _originalGravity, 0);
    }
}

}