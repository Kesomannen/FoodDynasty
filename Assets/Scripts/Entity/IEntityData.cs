using UnityEngine;

public interface IEntityData {
    string Name { get; }
    string Description { get; }
    Sprite Icon { get; }
}