using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public class InventoryItemData : ScriptableObject, IInfoProvider, IEntityData {
    [Header("Metadata")]
    [SerializeField] string _name;
    [ResizableTextArea]
    [FormerlySerializedAs("_flavorText")]
    [SerializeField] string _description;
    [ShowAssetPreview]
    [SerializeField] Sprite _image;

    [Header("Item")]
    [SerializeField] InventoryItemTier _tier;
    [SerializeField] double _price;
    
    public string Name => _name;
    public string Description => _description;
    public InventoryItemTier Tier => _tier;
    public double Price => _price;
    
    public virtual InventoryItemType Type => InventoryItemType.Other;

    public Sprite Image {
        get => _image;
        set => _image = value;
    }

    public virtual IEnumerable<(string Name, string Value)> GetInfo() {
        yield return ("Price", StringHelpers.FormatMoney(_price));
    }
}

public enum InventoryItemType {
    Other,
    BaseIngredient,
    Topping,
    Conveyor,
    Dispenser,
    Cooker,
    Seller,
    Modifier
}

public enum InventoryItemTier {
    Rusty
}

public interface IInfoProvider {
    IEnumerable<(string Name, string Value)> GetInfo();
}