using Dynasty.Grid;
using UnityEngine;

namespace Dynasty.Persistent {

[CreateAssetMenu(menuName = "Saving/Interpreter/Expansion")]
public class ExpansionSaveInterpreter : SaveInterpreter<int> {
    [SerializeField] GridExpansionManager _expansionManager;

    protected override int DefaultData => -1;

    protected override void OnLoad(int saveData) {
        while (_expansionManager.ExpansionIndex < saveData) {
            _expansionManager.Expand(false);
        }
    }

    protected override int GetSaveData() {
        return _expansionManager.ExpansionIndex;
    }
}

}