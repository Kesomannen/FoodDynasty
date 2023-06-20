using UnityEngine;

public class GridObjectControls : MonoBehaviour {
    [SerializeField] RectTransform _controlsParent;
    [SerializeField] GridObjectControl[] _controls;
    [SerializeField] GameEvent<GridObject> _showControlsEvent;

    void OnEnable() {
        _showControlsEvent.OnRaised += OnShowControls;
    }
    
    void OnDisable() {
        _showControlsEvent.OnRaised -= OnShowControls;
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