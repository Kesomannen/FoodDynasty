using System.Collections;
using UnityEngine;

public class Dispenser : MonoBehaviour {
    [SerializeField] DataObject<float> _spawnSpeed;
    [SerializeField] GameObject _prefab;
    [SerializeField] Transform _spawnPoint;
    [Space]
    [SerializeField] LocalConditional<bool> _condition;
    [SerializeField] LocalEvent<GameObject> _dispenseEvent;

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
}