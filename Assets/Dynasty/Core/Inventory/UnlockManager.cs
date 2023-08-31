using System;
using Dynasty.Library;
using Dynasty.Library.Events;
using UnityEngine;

namespace Dynasty.Core.Inventory {

[CreateAssetMenu(menuName = "Manager/Unlocks")]
public class UnlockManager : MonoScriptable {
    [SerializeField] MoneyManager _moneyManager;
    [SerializeField] ListEvent<ItemData> _unlockEvent;
    [SerializeField] UnlockData[] _unlockData;

    public int CurrentUnlockIndex { get; private set; }

    public override void OnAwake() {
        CurrentUnlockIndex = 0;
        _unlockEvent.Clear();
        
        _moneyManager.OnMoneyChanged += OnMoneyChanged;
        Update();
    }

    public override void OnDestroy() {
        _moneyManager.OnMoneyChanged -= OnMoneyChanged;
    }

    void OnMoneyChanged(double prev, double next) => Update();

    void Update() {
        if (CurrentUnlockIndex >= _unlockData.Length) return;
        var nextUnlock = _unlockData[CurrentUnlockIndex];
        if (_moneyManager.TotalMoneyMade < nextUnlock.MoneyMadeRequirement) return;
        
        foreach (var unlock in nextUnlock.Unlocks) {
            _unlockEvent.Add(unlock);
        }
        
        CurrentUnlockIndex++;
    }

    [Serializable]
    struct UnlockData {
        public double MoneyMadeRequirement;
        public ItemData[] Unlocks;
    }
}

}