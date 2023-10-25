using System;
using System.Linq;
using Dynasty.Core.Inventory;
using Dynasty.Machines;
using Dynasty.Library;
using TMPro;
using UnityEngine;

namespace Dynasty.UI.Components {

public class AutoBuyerItem : UIComponent<AutoBuyer> {
    [SerializeField] TMP_Dropdown _dropdown;
    [SerializeField] ListEvent<ItemData> _unlockedItems;

    AutoBuyer _autoBuyer;
    ItemData[] _items;
    
    public override void SetContent(AutoBuyer content) {
        _autoBuyer = content;
        _dropdown.ClearOptions();

        _items = _unlockedItems.Items
            .OrderBy(x => x is MachineItemData ? 1 : 0)
            .Prepend(null)
            .ToArray();
        
        _dropdown.AddOptions(_items
            .Select(x => x == null ? 
                new TMP_Dropdown.OptionData("None") : 
                new TMP_Dropdown.OptionData(x.Name, x.Icon)
            ).ToList()
        );
        
        _dropdown.value = _items.IndexOf(content.Item);
        _dropdown.onValueChanged.AddListener(ValueChanged);
    }

    void OnDisable() {
        _dropdown.onValueChanged.RemoveListener(ValueChanged);
    }
    
    void ValueChanged(int index) {
        _autoBuyer.Item = _items[index];
    }
}

}