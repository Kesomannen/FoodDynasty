using Dynasty.Food.Data;
using Dynasty.Library;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dynasty.Food.Instance {

[RequireComponent(typeof(FoodBehaviour), typeof(Rigidbody))]
public class StabilityBehaviour : MonoBehaviour {
    [Range(0, 1)]
    [SerializeField] float _randomness;
    [SerializeField] FoodTraitSelection _stabilityTrait;
    [SerializeField] bool _useSparseUpdate = true;
    [Space]
    [SerializeField] AnimationCurve _velocityStabilityLossCurve;
    [SerializeField] Vector2 _velocityRange;
    [AllowNesting]
    [SerializeField] [ReadOnly] float _velocity;
    [AllowNesting]
    [SerializeField] [ReadOnly] float _current;

    FoodBehaviour _food;
    Rigidbody _rigidbody;
    float _randomMultiplier;
    
    void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        _food = GetComponent<FoodBehaviour>();
    }

    void OnEnable() {
        _randomMultiplier = Random.Range(1 - _randomness, 1 + _randomness);
        TickManager.AddListener(OnTick, _useSparseUpdate);
    }
    
    void OnDisable() {
        TickManager.RemoveListener(OnTick, _useSparseUpdate);
    }

    void OnTick(float delta) {
        _current = _food.GetTrait<float>(_stabilityTrait.Hash);
        _velocity = _rigidbody.velocity.magnitude;

        var velocity = Mathf.InverseLerp(_velocityRange.x, _velocityRange.y, _velocity);
        var velocityLoss = _velocityStabilityLossCurve.Evaluate(velocity);

        var newStability = _current - velocityLoss * delta * _randomMultiplier;
        _food.SetTrait(_stabilityTrait.Hash, newStability);

        if (newStability > 0) return;
        Debug.Log("Food has fallen over!");
        _food.Dispose();
    }
}

}