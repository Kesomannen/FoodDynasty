using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Dynasty.UI.Miscellanious {

[RequireComponent(typeof(TMP_Dropdown))]
public class EnumDropdown<T> : MonoBehaviour where T : Enum {
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

    void SetOptions() {
        _dropdown = GetComponent<TMP_Dropdown>();
        _dropdown.ClearOptions();

        var options = Enum.GetNames(typeof(T)).Select(e => new TMP_Dropdown.OptionData(e)).ToList();
        _dropdown.AddOptions(options);
    }
}

}