using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace Dynasty.UI.Miscellanious {

[RequireComponent(typeof(TMP_Dropdown))]
public class EnumDropdown<T> : MonoBehaviour where T : Enum {
    [SerializeField] bool _overrideNames;
    [SerializeField] string[] _enumNames;
    [SerializeField] string[] _nameOverrides;
    
    TMP_Dropdown _dropdown;

    public T Value {
        get => (T) (object) _dropdown.value;
        set => _dropdown.value = (int) (object) value;
    }

    public event Action<T> OnValueChanged; 

    void Awake() {
        SetOptions();
        
        _dropdown.onValueChanged.AddListener(_ => OnValueChanged?.Invoke(Value));
    }

    void Reset() {
        SetOptions();
    }

    void OnValidate() {
        if (_overrideNames) {
            _enumNames = Enum.GetNames(typeof(T));
            if (_nameOverrides.Length != _enumNames.Length) {
                _nameOverrides = _enumNames;
            }
            SetOptions();
        } else {
            _enumNames = null;
        }
    }

    void SetOptions() {
        _dropdown = GetComponent<TMP_Dropdown>();
        _dropdown.ClearOptions();

        var options = Enum.GetNames(typeof(T)).Select(e => _overrideNames ? _nameOverrides[(int)Enum.Parse(typeof(T), e)] : e);
        _dropdown.AddOptions(options.ToList());
    }
}

}