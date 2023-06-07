using UnityEngine;
using UnityEngine.EventSystems;

public interface ISupply {
    int MaxSupply { get; set; }
    int CurrentSupply { get; set; }
}

public class Supply : MonoBehaviour, ISupply, IPointerClickHandler {
    [SerializeField] Optional<int> _maxSupply;
    [SerializeField] LocalConditional<bool> _condition;
    [SerializeField] GenericLocalEvent _useEvent;

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
    void OnDispensed() => CurrentSupply--;
    
    void OnEnable() {
        _condition.AddCondition(HasSupply);
        _useEvent.AddListener(OnDispensed);
    }
    
    void OnDisable() {
        _condition.RemoveCondition(HasSupply);
        _useEvent.RemoveListener(OnDispensed);
    }

    public void OnPointerClick(PointerEventData eventData) {
        _currentSupply = MaxSupply;
    }
}