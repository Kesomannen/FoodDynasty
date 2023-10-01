using Dynasty.Core.Inventory;
using UnityEngine;

namespace Dynasty.Persistent.Mapping {

[CreateAssetMenu(menuName = "Saving/Interpreter/Unlock")]
public class UnlockSaveInterpreter : SaveInterpreter<int> {
    [SerializeField] UnlockManager _unlockManager;

    protected override int DefaultData => -1;

    protected override void OnLoad(int saveData) {
        while (_unlockManager.CurrentUnlockIndex < saveData) {
            _unlockManager.UnlockNext(false);
        }
    }

    protected override int GetSaveData() {
        return _unlockManager.CurrentUnlockIndex;
    }
}

}