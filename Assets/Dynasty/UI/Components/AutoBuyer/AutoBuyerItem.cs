using System.Linq;
using Dynasty.Core.Inventory;
using Dynasty.Library.Events;
using Dynasty.Machines;
using Dynasty.Library.Extensions;
using TMPro;
using UnityEngine;

namespace Dynasty.UI.Components {

public class AutoBuyerItem : UIComponent<AutoBuyer> {
    [SerializeField] TMP_Dropdown _dropdown;
    [SerializeField] ListEvent<ItemData> _unlockedItems;

    public override void SetContent(AutoBuyer content) {
        _dropdown.ClearOptions();

        var items = _unlockedItems.Items
            .OrderBy(x => x is MachineItemData ? 1 : 0)
            .Prepend(null)
            .ToArray();
        
        _dropdown.AddOptions(items
            .Select(x => x == null ? 
                new TMP_Dropdown.OptionData("None") : 
                new TMP_Dropdown.OptionData(x.Name, x.Icon)
            ).ToList()
        );
        
        _dropdown.value = items.IndexOf(content.Item);
        _dropdown.onValueChanged.AddListener(ValueChanged);
        return;

        void ValueChanged(int index) {
            content.Item = items[index];
        }
    }
}

}