using Dynasty;
using UnityEngine;

namespace Dynasty.UI.Components {

public class ItemDataComponent : UIComponent<Item> {
    [SerializeField] Container<ItemData> _dataContainer;

    public override void SetContent(Item content) {
        _dataContainer.SetContent(content.Data);
    }
}

}