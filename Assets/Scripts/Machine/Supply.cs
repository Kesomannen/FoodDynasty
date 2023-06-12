using System.Collections.Generic;
using UnityEngine;

public interface ISupply {
    int MaxSupply { get; set; }
    int CurrentSupply { get; set; }
    
    bool IsValidRefill(InventoryItemData item);
}

public class Supply : MonoBehaviour, ISupply, IInfoProvider {
    [SerializeField] Optional<int> _maxSupply;
    [SerializeField] InventoryItemData _refillItem;
    [SerializeField] CheckEvent<bool> _condition;
    [SerializeField] GenericEvent _onUsed;

    int _currentSupply;

    public int MaxSupply {
        get => _maxSupply.Enabled ? _maxSupply.Value : int.MaxValue;
        set => _maxSupply = new Optional<int>(value);
    }
    
    public int CurrentSupply {
        get => _currentSupply;
        set => _currentSupply = Mathf.Clamp(value, 0, MaxSupply);
    }

    public bool IsValidRefill(InventoryItemData item) => _refillItem == item;
    bool HasSupply() => CurrentSupply > 0;
    void OnUsed() => CurrentSupply--;
    
    void OnEnable() {
        _condition.AddCondition(HasSupply);
        _onUsed.OnRaisedGeneric += OnUsed;
    }
    
    void OnDisable() {
        _condition.RemoveCondition(HasSupply);
        _onUsed.OnRaisedGeneric -= OnUsed;
    }

    public IEnumerable<(string Name, string Value)> GetInfo() {
        if (_maxSupply.Enabled) {
            yield return ("Max Supply", _maxSupply.Value.ToString());
        }
    }
}