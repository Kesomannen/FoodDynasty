using UnityEngine;

namespace Dynasty.Library {

public interface IEntityData {
    string Name { get; }
    string Description { get; }
    string ShortDescription { get; }
    Sprite Icon { get; }
}

}