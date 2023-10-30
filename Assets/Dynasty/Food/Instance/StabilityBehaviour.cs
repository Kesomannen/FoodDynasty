using Dynasty.Food;
using Dynasty.Library;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dynasty.Food {

[RequireComponent(typeof(FoodBehaviour))]
public class StabilityBehaviour : MonoBehaviour {
    [Range(0, 1)]
    [SerializeField] float _randomness;
    [SerializeField] FoodTraitSelection _stabilityTrait;
    [SerializeField] bool _useSparseUpdate = true;
    [SerializeField] float _stabilityLoss;
    [SerializeField] GenericEvent _onFallenOver;
    [Space]
    [AllowNesting]
    [SerializeField] [ReadOnly] Modifier _current;
    [SerializeField] [ReadOnly] float _currentDelta;

    FoodBehaviour _food;
    float _randomMultiplier;
    bool _fallenOver;
    
    void Awake() {
        _food = GetComponent<FoodBehaviour>();
    }

    void OnEnable() {
        _fallenOver = false;
        _randomMultiplier = Random.Range(1 - _randomness, 1 + _randomness);
        TickManager.AddListener(OnTick, _useSparseUpdate);
    }
    
    void OnDisable() {
        TickManager.RemoveListener(OnTick, _useSparseUpdate);
    }

    void OnTick(float delta) {
        if (_fallenOver) return;
        
        _current = _food.GetTrait<Modifier>(_stabilityTrait.Hash);

        var newStability = _current - new Modifier(additive: _stabilityLoss * delta * _randomMultiplier);
        _food.SetTrait(_stabilityTrait.Hash, newStability);

        _currentDelta = newStability.DeltaFloat;
        if (_currentDelta > 0) return;

        if (_onFallenOver != null) {
            _onFallenOver.Raise();
        }
        _fallenOver = true;
        
        _food.Dispose();
    }
}

}