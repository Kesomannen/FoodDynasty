using UnityEngine;

namespace Dynasty.Library.Entity {

/// <summary>
/// MonoBehaviour implementation for IEntityData for serialization purposes.
/// </summary>
/// <remarks>Not abstract to be available in GenericUnityObjects</remarks>
public class Entity : MonoBehaviour, IEntityData {
    public virtual string Name { get; }
    public virtual string Description { get; }
    public virtual string ShortDescription { get; }
    public virtual Sprite Icon { get; }
}

}