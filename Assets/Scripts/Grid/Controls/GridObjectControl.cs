using System.Collections.Generic;
using UnityEngine;

public abstract class GridObjectControl : ScriptableObject {
    public abstract bool GetControls(GridObject gridObject, out IEnumerable<Transform> uiElements);
}