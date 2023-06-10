using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface ISupply {
    int MaxSupply { get; set; }
    int CurrentSupply { get; set; }
}

public class Supply : MonoBehaviour, ISupply, IPointerClickHandler, IInfoProvider {
    [SerializeField] Optional<int> _maxSupply;
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

    public void OnPointerClick(PointerEventData eventData) {
        _currentSupply = MaxSupply;
    }

    public IEnumerable<(string Name, string Value)> GetInfo() {
        if (_maxSupply.Enabled) {
            yield return ("Max Supply", _maxSupply.Value.ToString());
        }
    }
}