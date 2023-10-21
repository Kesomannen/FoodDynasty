using Dynasty.Food;
using Dynasty.Library.Pooling;
using UnityEngine;

namespace Dynasty.Food {

[CreateAssetMenu(menuName = "Pooling/Food")]
public class FoodObjectPool : CustomObjectPool<FoodBehaviour> {
    public bool Spawn(out FoodBehaviour food) {
        food = null;
        var manager = FoodManager.Singleton;
        if (manager == null) return false;

        food = Get();
        if (manager.Add(food)) return true;
        
        Release(food);
        return false;
    }
}

}