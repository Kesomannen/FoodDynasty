using System;
using System.Linq;
using Dynasty.Core.Tooltip;
using Dynasty.Library;
using Dynasty.Library.Events;
using UnityEngine;

namespace Dynasty.Core.Inventory {

[CreateAssetMenu(menuName = "Manager/Unlocks")]
public class UnlockManager : MonoScriptable {
    [SerializeField] MoneyManager _moneyManager;
    [SerializeField] ListEvent<ItemData> _unlockEvent;
    [SerializeField] GameEvent<PopupData> _popupEvent;
    [SerializeField] UnlockData[] _unlockData;

    public int CurrentUnlockIndex { get; private set; }

    public override void OnAwake() {
        CurrentUnlockIndex = -1;
        _unlockEvent.Clear();
        _moneyManager.OnMoneyChanged += OnMoneyChanged;
    }

    public override void OnDestroy() {
        _moneyManager.OnMoneyChanged -= OnMoneyChanged;
    }

    void OnMoneyChanged(double prev, double next) {
        Update();
    }

    void Update() {
        if (CurrentUnlockIndex >= _unlockData.Length) return;
        var nextUnlock = _unlockData[CurrentUnlockIndex + 1];
        if (_moneyManager.TotalMoneyMade < nextUnlock.MoneyMadeRequirement) return;

        UnlockNext(true);
    }

    public void UnlockNext(bool showPopup) {
        var unlock = _unlockData[CurrentUnlockIndex + 1];
        foreach (var item in unlock.Unlocks) {
            _unlockEvent.Add(item);
        }
        
        if (showPopup) {
            var itemList = string.Join("\n", unlock.Unlocks.Select(u => $"- {u.Name}"));
            
            _popupEvent.Raise(new PopupData {
                Header = $"You unlocked {unlock.Header}!",
                Body = $"You have unlocked the following items:\n{itemList}",
                Actions = new[] {
                    PopupAction.Positive("Ok")
                }
            });
        }

        CurrentUnlockIndex++;
    }

    [Serializable]
    struct UnlockData {
        public string Header;
        public double MoneyMadeRequirement;
        public ItemData[] Unlocks;
    }
    
}

}