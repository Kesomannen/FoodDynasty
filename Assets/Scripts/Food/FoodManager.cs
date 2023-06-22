using System;
using UnityEngine;

public class FoodManager : MonoBehaviour {
    [SerializeField] CustomObjectPool<Food> _foodPool;

    FoodData[] _foodData;
    Food[] _instances;
    
    int _maxFoods;
    int _currentId;

    void Awake() {
        _maxFoods = _foodPool.MaxSize;
        _foodData = new FoodData[_maxFoods];
        _instances = new Food[_maxFoods];
    }

    public Food Spawn(FoodData data) {
        if (_currentId >= _maxFoods) {
            throw new Exception("Max foods reached");
        }
        
        var food = _foodPool.Get();
        
        food.Id = _currentId++;
        _instances[food.Id] = food;
        _foodData[food.Id] = data;

        return food;
    }

    public void Destroy(int id) {
        var instance = _instances[id];
        instance.Dispose();
        
        ReplaceWithLast(id);
    }

    void ReplaceWithLast(int id) {
        var lastId = _currentId - 1;
        
        _instances[id] = _instances[lastId];
        _foodData[id] = _foodData[lastId];
        _instances[id].Id = id;

        _currentId--;
    }
}