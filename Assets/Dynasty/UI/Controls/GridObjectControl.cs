using System.Collections.Generic;
using Dynasty.Grid;
using UnityEngine;

namespace Dynasty.UI.Controls {

public abstract class GridObjectControl : ScriptableObject {
    public abstract bool GetControls(GridObject gridObject, out IEnumerable<Transform> uiElements);
}

}