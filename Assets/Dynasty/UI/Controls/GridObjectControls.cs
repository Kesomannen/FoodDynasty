using System.Collections.Generic;
using Dynasty.Core.Grid;
using Dynasty.Library.Events;
using UnityEngine;

namespace Dynasty.UI.Controls {

public class GridObjectControls : MonoBehaviour {
    [SerializeField] RectTransform _controlsParent;
    [SerializeField] GridObjectControl[] _controls;
    [SerializeField] GameEvent<GridObject> _showControlsEvent;

    void OnEnable() {
        _showControlsEvent.AddListener(OnShowControls);
    }
    
    void OnDisable() {
        _showControlsEvent.RemoveListener(OnShowControls);
    }

    void OnShowControls(GridObject gridObject) {
        _controlsParent.gameObject.SetActive(false);
        
        var elementCount = 0;
        foreach (var control in _controls) {
            if (!control.GetControls(gridObject, out var uiElements)) continue;
            foreach (var element in uiElements) {
                element.transform.SetParent(_controlsParent, false);
                elementCount++;
            }
        }
        
        if (elementCount == 0) return;
        _controlsParent.gameObject.SetActive(true);
    }
}

}