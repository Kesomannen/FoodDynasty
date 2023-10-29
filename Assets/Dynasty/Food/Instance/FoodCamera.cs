using System.Linq;
using Dynasty.Library;
using UnityEngine;

namespace Dynasty.Food {

[RequireComponent(typeof(Camera))]
public class FoodCamera : MonoBehaviour {
    [SerializeField] Vector3 _offset;
    
    Camera _camera;
    bool _isActive;

    FoodBehaviour _newestFood;
    
    FoodBehaviour _currentFood;
    Rigidbody _currentRigidbody;
    
    static FoodManager FoodManager => FoodManager.Singleton;

    public bool IsActive {
        get => _isActive;
        set {
            _isActive = value;
            if (_isActive) Activate();
            else Deactivate();
        }
    }

    void Awake() {
        _camera = GetComponent<Camera>();
    }

    void Start() {
        FoodManager.OnFoodAdded += OnFoodAdded;
    }

    void OnDestroy() {
        if (FoodManager != null) {
            FoodManager.OnFoodAdded -= OnFoodAdded;
        }
    }

    void Activate() {
        _camera.enabled = true;
        AttachToNewFood();
    }

    void Deactivate() {
        _camera.enabled = false;
        transform.SetParent(null, false);
        
        if (_currentFood != null) {
            _currentFood.OnDisposed -= OnFoodDisposed;
        }
    }

    void AttachToNewFood() {
        if (_currentFood != null) {
            _currentFood.OnDisposed -= OnFoodDisposed;
        }
        
        var t = transform;
        _currentFood = _newestFood != null ? _newestFood : FoodManager.Food.ToArray().GetRandom();
        
        t.SetParent(_currentFood.transform, false);
        t.localPosition = _offset;
        _currentRigidbody = _currentFood.GetComponent<Rigidbody>();
        
        _currentFood.OnDisposed += OnFoodDisposed;
    }
    
    void OnFoodDisposed(FoodBehaviour food) {
        AttachToNewFood();
    }

    void OnFoodAdded(FoodBehaviour food) {
        _newestFood = food;
    }

    void Update() {
        if (!IsActive) return;
        transform.forward = _currentRigidbody.velocity;
    }
}

}