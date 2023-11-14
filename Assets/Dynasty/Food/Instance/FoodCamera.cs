using System;
using System.Linq;
using Dynasty.Library;
using UnityEngine;

namespace Dynasty.Food {

[RequireComponent(typeof(Camera))]
public class FoodCamera : MonoBehaviour {
    [SerializeField] Vector3 _offset;
    [SerializeField] float _rotationSpeed;
    
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
    
    public bool IsAvailable { get; private set; }
    
    public event Action<bool> OnAvailabilityChanged; 

    void Awake() {
        _camera = GetComponent<Camera>();
    }

    void Start() {
        FoodManager.OnFoodAdded += OnFoodAdded;
        FoodManager.OnFoodRemoved += OnFoodRemoved;
    }

    void OnDestroy() {
        if (FoodManager != null) {
            FoodManager.OnFoodAdded -= OnFoodAdded;
            FoodManager.OnFoodRemoved -= OnFoodRemoved;
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
            _currentFood.OnDisposed -= OnCurrentFoodDisposed;
        }
    }

    void AttachToNewFood() {
        if (_currentFood != null) {
            _currentFood.OnDisposed -= OnCurrentFoodDisposed;
        }
        
        if (!IsAvailable) return;
        
        var t = transform;
        _currentFood = _newestFood != null ? _newestFood : FoodManager.Food.ToArray().GetRandom();
        
        t.SetParent(_currentFood.transform, false);
        t.localPosition = _offset;
        _currentRigidbody = _currentFood.GetComponent<Rigidbody>();
        
        _currentFood.OnDisposed += OnCurrentFoodDisposed;
    }
    
    void OnCurrentFoodDisposed(FoodBehaviour food) {
        AttachToNewFood();
    }

    void OnFoodAdded(FoodBehaviour food) {
        _newestFood = food;
        
        if (!IsAvailable) {
            IsAvailable = true;
            OnAvailabilityChanged?.Invoke(true);
        }
    }
    
    void OnFoodRemoved(FoodBehaviour food) {
        if (_newestFood != food) return;
        
        _newestFood = null;
        IsActive = false;
        IsAvailable = false;
        OnAvailabilityChanged?.Invoke(false);
    }

    void Update() {
        if (!IsActive) return;
        var targetRotation = Quaternion.LookRotation(_currentRigidbody.velocity.normalized);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
    }
}

}