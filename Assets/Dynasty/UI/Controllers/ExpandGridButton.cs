using System;
using Dynasty.Grid;
using Dynasty.Library;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dynasty.UI.Controllers {

[RequireComponent(typeof(Button))]
public class ExpandGridButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] TMP_Text _label;
    [SerializeField] GridExpansionController _controller;
    [SerializeField] GridExpansionManager _manager;
    [SerializeField] MoneyManager _moneyManager;

    Button _button;

    void Awake() {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => _manager.Expand(true));
    }

    void OnEnable() {
        _moneyManager.OnMoneyChanged += OnMoneyChanged;
        _manager.OnExpansionChanged += OnExpansionChanged;
        UpdateText();
    }
    
    void OnDisable() {
        _moneyManager.OnMoneyChanged -= OnMoneyChanged;
        _manager.OnExpansionChanged -= OnExpansionChanged;
    }
    
    void OnMoneyChanged(double prev, double next) {
        UpdateText();
    }
    
    void OnExpansionChanged(Vector2Int size, bool animate) {
        UpdateText();
    }

    void UpdateText() {
        if (_manager.TryGetNextExpansion(out var next)) {
            _label.text = $"Expand build area ({StringHelpers.FormatMoney(next.Cost)})";
            _button.interactable = _moneyManager.CurrentMoney >= next.Cost;
        } else {
            _label.text = "No more expansions available";
            _button.interactable = false;
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData) {
        _controller.StartPreviewing();
    }

    public void OnPointerExit(PointerEventData eventData) {
        _controller.StopPreviewing();
    }
}

}