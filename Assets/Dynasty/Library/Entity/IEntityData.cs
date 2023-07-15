using UnityEngine;

namespace Dynasty.Library.Entity {

public interface IEntityData {
    string Name { get; }
    string Description { get; }
    Sprite Icon { get; }
}

}