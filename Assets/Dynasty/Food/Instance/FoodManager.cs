using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Dynasty.Food {

public class FoodManager : MonoBehaviour {
    [SerializeField] int _maxFood = 1000;

    [CanBeNull]
    public static FoodManager Singleton { get; private set; }
    
    readonly HashSet<FoodBehaviour> _food = new();

    void Awake() {
        if (Singleton != null) {
            Destroy(gameObject);
            return;
        }

        Singleton = this;
    }

    void OnDestroy() {
        if (Singleton == this) {
            Singleton = null;
        }
    }

    public bool Add(FoodBehaviour food) {
        if (_food.Count >= _maxFood || !_food.Add(food)) return false;
        food.OnDisposed += OnDisposed;
        return true;
    }

    public bool Remove(FoodBehaviour obj) {
        if (!_food.Remove(obj)) return false;
        obj.OnDisposed -= OnDisposed;
        return false;
    }
    
    void OnDisposed(FoodBehaviour food) {
        Remove(food);
    }

    public IEnumerable<FoodBehaviour> GetAllFood() {
        return _food;
    }
}

}