using Dynasty;
using UnityEngine;

namespace Dynasty.Persistent {

[CreateAssetMenu(menuName = "Saving/Interpreter/Unlock")]
public class UnlockSaveInterpreter : SaveInterpreter<int> {
    [SerializeField] UnlockManager _unlockManager;

    protected override int DefaultData => 0;

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