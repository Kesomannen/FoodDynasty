using Dynasty.Core.Grid;
using Dynasty.Library;
using Dynasty.UI.Misc;
using UnityEngine;

namespace Dynasty.UI.Controls {

public class GridObjectControls : MonoBehaviour {
    [SerializeField] WorldCanvasWindow _window;
    [SerializeField] GridObjectControl[] _controls;
    [SerializeField] GameEvent<GridObject> _showControlsEvent;

    void OnEnable() {
        _showControlsEvent.AddListener(OnShowControls);
    }
    
    void OnDisable() {
        _showControlsEvent.RemoveListener(OnShowControls);
    }

    void OnShowControls(GridObject gridObject) {
        _window.gameObject.SetActive(false);
        
        var elementCount = 0;
        foreach (var control in _controls) {
            if (!control.GetControls(gridObject, out var uiElements)) continue;
            foreach (var element in uiElements) {
                element.transform.SetParent(_window.transform, false);
                elementCount++;
            }
        }
        
        if (elementCount == 0) return;
        _window.Show(gridObject.transform.position);
    }
}

}