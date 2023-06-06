using UnityEngine;

public class Item : MonoBehaviour {
    [SerializeField] float _baseSellPrice;

    public float BaseSellPrice => _baseSellPrice;
    public Modifier SellPriceModifier { get; set; } = new(multiplicative: 1f);
}