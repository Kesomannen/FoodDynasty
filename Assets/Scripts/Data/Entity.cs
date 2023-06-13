using UnityEngine;

public class Entity : MonoBehaviour, IEntityData {
    public virtual string Name => string.Empty;
    public virtual string Description => string.Empty;
}