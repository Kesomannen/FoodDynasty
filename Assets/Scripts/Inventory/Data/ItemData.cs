using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public class ItemData : ScriptableObject, IInfoProvider, IEntityData {
    [Header("Metadata")]
    [SerializeField] string _name;
    [ResizableTextArea]
    [FormerlySerializedAs("_flavorText")]
    [SerializeField] string _description;
    [ShowAssetPreview]
    [SerializeField] Sprite _image;

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

    public ItemTier Tier {
        get => _tier;
        set => _tier = value;
    }

    public double Price {
        get => _price;
        set => _price = value;
    }

    public virtual ItemType Type => ItemType.Other;

    public Sprite Image {
        get => _image;
        set => _image = value;
    }

    public virtual IEnumerable<(string Name, string Value)> GetInfo() {
        yield return ("Price", StringHelpers.FormatMoney(_price));
    }
}

public enum ItemType {
    Other,
    BaseIngredient,
    Topping,
    Conveyor,
    Dispenser,
    Cooker,
    Seller,
    Modifier
}

public enum ItemTier {
    Rusty,
    Metallic
}

public interface IInfoProvider {
    IEnumerable<(string Name, string Value)> GetInfo();
}