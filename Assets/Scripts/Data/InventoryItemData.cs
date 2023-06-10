using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class InventoryItemData : ScriptableObject, IInfoProvider {
    [Header("Metadata")]
    [SerializeField] string _name;
    [ResizableTextArea]
    [SerializeField] string _flavorText;
    [ShowAssetPreview]
    [SerializeField] Sprite _image;

    [Header("Item")]
    [SerializeField] InventoryItemType _type;
    [SerializeField] InventoryItemTier _tier;
    [SerializeField] double _price;
    
    public string Name => _name;
    public string FlavorText => _flavorText;
    public InventoryItemType Type => _type;
    public InventoryItemTier Tier => _tier;
    public double Price => _price;

    public Sprite Image {
        get => _image;
        set => _image = value;
    }

    public virtual IEnumerable<(string Name, string Value)> GetInfo() {
        yield return ("Price", StringUtil.FormatMoney(_price));
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