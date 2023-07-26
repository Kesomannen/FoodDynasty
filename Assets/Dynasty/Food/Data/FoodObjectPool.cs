using Dynasty.Food.Instance;
using Dynasty.Library.Pooling;
using UnityEngine;

namespace Dynasty.Food.Data {

[CreateAssetMenu(menuName = "Pooling/Food")]
public class FoodObjectPool : CustomObjectPool<FoodBehaviour> {
    public bool Spawn(out FoodBehaviour food) {
        food = null;
        var manager = FoodManager.Singleton;
        if (manager== null) return false;

        food = Get();
        if (manager.Add(food)) return true;
        
        Release(food);
        return false;
    }
}

}