using UnityEngine;

public abstract class FoodModelProvider : MonoBehaviour {
    public abstract void SetBaseModel(Poolable model);
    public abstract void AddToppingModel(Poolable model);
    public abstract void ReturnOriginalModel();
}