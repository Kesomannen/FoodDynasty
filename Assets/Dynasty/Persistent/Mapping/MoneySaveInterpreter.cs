using Dynasty.Core.Inventory;
using Dynasty.Persistent.Core;
using UnityEngine;

namespace Dynasty.Persistent.Mapping {

[CreateAssetMenu(menuName = "Saving/Interpreter/Money")]
public class MoneySaveInterpreter : SaveInterpreter<double> {
    [SerializeField] double _startingMoney;
    [SerializeField] MoneyManager _moneyManager;

    protected override double DefaultValue => _startingMoney;

    protected override void OnAfterLoad(double saveData) {
        _moneyManager.CurrentMoney = saveData;
    }

    protected override double GetSaveData() {
        return _moneyManager.CurrentMoney;
    }
}

}