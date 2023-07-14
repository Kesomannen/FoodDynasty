using UnityEngine;

public class Entity : MonoBehaviour, IEntityData {
    public virtual string Name { get; }
    public virtual string Description { get; }
    public virtual Sprite Icon { get; }
}