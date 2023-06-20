using System;
using UnityEngine;

public class InventoryItemDataBuyControl : MonoBehaviour {
    [SerializeField] Container<InventoryItemData> _container;
    [SerializeField] NumberInputController _numberInput;
    [SerializeField] NumberInputController.ModifyMode _modifyMode;

    Action<int> _callback;
    
    public void Initialize(InventoryItemData content, MoneyManager moneyManager, Action<int> callback) {
        _callback = callback;
        
        _numberInput.Initialize(
            startValue: 1,
            maxValue: () => {
                var max = moneyManager.CurrentMoney / content.Price;
                max = Math.Floor(max);
                
                if (max > int.MaxValue) return int.MaxValue;
                return (int) max;
            },
            mode: _modifyMode
        );
        
        _container.SetContent(content); ;
    }

    public void Buy() {
        _callback?.Invoke((int) _numberInput.Value);
    }
}