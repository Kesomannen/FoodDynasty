using Dynasty.Library;
using UnityEngine;

namespace Dynasty.UI.Tutorial {

[CreateAssetMenu(menuName = "Tutorial/Money Mission")]
public class MoneyMission : Mission {
    [SerializeField] float _goal;
    [SerializeField] MoneyManager _moneyManager;
    
    public override float Goal => _goal;
    
    double _startingMoney;

    public override string ProgressText => $"{StringHelpers.FormatMoney(Progress)}/{StringHelpers.FormatMoney(Goal)}";

    public override void Start() {
        _startingMoney = _moneyManager.TotalMoneyMade;
        _moneyManager.OnMoneyChanged += OnMoneyChanged;
    }

    protected override void OnComplete() {
        _moneyManager.OnMoneyChanged -= OnMoneyChanged;
    }
    
    void OnMoneyChanged(double prev, double current) {
        Progress = (float) (_moneyManager.TotalMoneyMade - _startingMoney);
    }
}

}