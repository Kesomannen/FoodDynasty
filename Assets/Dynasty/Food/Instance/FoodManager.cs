using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Dynasty.Food {

public class FoodManager : MonoBehaviour {
    public static FoodManager Singleton { get; private set; }
    
    readonly HashSet<FoodBehaviour> _food = new();
    
    public IEnumerable<FoodBehaviour> Food => _food;
    public int FoodCount => _food.Count;
    
    public event Action OnFoodChanged;
    public event Action<FoodBehaviour> OnFoodAdded, OnFoodRemoved; 

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
        if (!_food.Add(food)) return false;
        food.OnDisposed += OnDisposed;
        
        OnFoodAdded?.Invoke(food);
        OnFoodChanged?.Invoke();
        return true;
    }

    public bool Remove(FoodBehaviour obj) {
        if (!_food.Remove(obj)) return false;
        obj.OnDisposed -= OnDisposed; 
        
        OnFoodRemoved?.Invoke(obj);
        OnFoodChanged?.Invoke();
        return true;
    }
    
    void OnDisposed(FoodBehaviour food) {
        Remove(food);
    }
}

}