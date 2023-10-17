using Dynasty.Food;
using Dynasty.Library;
using Dynasty.Library.Events;
using Dynasty.Library.Helpers;
using Dynasty.Library.Pooling;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dynasty.Food {

[RequireComponent(typeof(FoodBehaviour), typeof(Rigidbody))]
public class StabilityBehaviour : MonoBehaviour {
    [Range(0, 1)]
    [SerializeField] float _randomness;
    [SerializeField] FoodTraitSelection _stabilityTrait;
    [SerializeField] bool _useSparseUpdate = true;
    [Space]
    [SerializeField] AnimationCurve _velocityStabilityLossCurve;
    [SerializeField] Vector2 _velocityRange;
    [SerializeField] GenericEvent _onFallenOver;
    [Space]
    [AllowNesting]
    [SerializeField] [ReadOnly] float _velocity;
    [AllowNesting]
    [SerializeField] [ReadOnly] float _current;

    FoodBehaviour _food;
    Rigidbody _rigidbody;
    float _randomMultiplier;
    bool _fallenOver;
    
    void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
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
        
        _current = _food.GetTrait<float>(_stabilityTrait.Hash);
        _velocity = _rigidbody.velocity.magnitude;

        var velocity = Mathf.InverseLerp(_velocityRange.x, _velocityRange.y, _velocity);
        var velocityLoss = _velocityStabilityLossCurve.Evaluate(velocity);

        var newStability = _current - velocityLoss * delta * _randomMultiplier;
        _food.SetTrait(_stabilityTrait.Hash, newStability);

        if (newStability > 0) return;

        if (_onFallenOver != null) {
            _onFallenOver.Raise();
        }
        _fallenOver = true;
        
        _food.Dispose();
    }
}

}