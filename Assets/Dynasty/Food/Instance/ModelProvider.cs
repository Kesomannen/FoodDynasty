using System.Collections.Generic;
using Dynasty.Library.Pooling;
using UnityEngine;

namespace Dynasty.Food {

public abstract class ModelProvider : MonoBehaviour {
    public abstract void SetBaseModel(Poolable model);
    public abstract void AddToppingModel(Poolable model);
    public abstract void ReturnOriginalModel();

    public abstract IEnumerable<Poolable> GetToppings();
}

}