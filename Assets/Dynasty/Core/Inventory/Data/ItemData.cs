using System.Collections.Generic;
using Dynasty.Library;
using Dynasty.Library.Helpers;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dynasty.Core.Inventory {

/// <summary>
/// Base class for items.
/// </summary>
public class ItemData : ScriptableObject, IEntityData, IInfoProvider {
    [Header("Metadata")]
    [SerializeField] string _name;
    
    [ResizableTextArea]
    [FormerlySerializedAs("_flavorText")]
    [SerializeField] string _description;
    [SerializeField] string _shortDescription;
    
    [ShowAssetPreview]
    [SerializeField] Sprite _icon;

    [Header("Item")]
    [SerializeField] ItemTier _tier;
    [SerializeField] double _price;

    public string Name {
        get => _name;
        set => _name = value;
    }

    public string Description {
        get => _description;
        set => _description = value;
    }
    
    public string ShortDescription {
        get => _shortDescription;
        set => _shortDescription = value;
    }

    public ItemTier Tier {
        get => _tier;
        set => _tier = value;
    }

    public double Price {
        get => _price;
        set => _price = value;
    }

    public virtual ItemType Type => ItemType.Other;

    public Sprite Icon {
        get => _icon;
        set => _icon = value;
    }

    public virtual IEnumerable<EntityInfo> GetInfo() {
        yield return new EntityInfo("Price", StringHelpers.FormatMoney(_price));
    }
}

}

public enum ItemType {
    Other,
    Food,
    Topping,
    Conveyor,
    Dispenser,
    Cooker,
    Seller,
    Modifier,
}

public enum ItemTier {
    Rusty,
    Metallic
}