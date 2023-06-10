using TMPro;
using UnityEngine;

public class InfoContainer : MonoBehaviour {
    [SerializeField] TMP_Text _nameText;
    [SerializeField] TMP_Text _valueText;
    
    public void SetContent((string Name, string Value) content) {
        _nameText.text = content.Name;
        _valueText.text = content.Value;
    }
}