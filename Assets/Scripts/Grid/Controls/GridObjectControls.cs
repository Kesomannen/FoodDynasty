using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridObjectControls : MonoBehaviour {
    [SerializeField] RectTransform _controlsParent;
    [SerializeField] GridObjectControl[] _controls;
    [Space]
    [SerializeField] GameEvent<GridObject> _showControlsEvent;

    void OnEnable() {
        _showControlsEvent.OnRaised += OnShowControls;
        _controlsParent.gameObject.SetActive(false);
    }
    
    void OnDisable() {
        _showControlsEvent.OnRaised -= OnShowControls;
    }

    void OnShowControls(GridObject gridObject) {
        var elementCount = 0;
        foreach (var control in _controls) {
            if (!control.GetControls(gridObject, out var uiElements)) continue;
            foreach (var element in uiElements) {
                element.transform.SetParent(_controlsParent, false);
                elementCount++;
            }
        }
        
        if (elementCount == 0) return;
        
        _controlsParent.transform.position = MouseHelpers.MouseScreenPosition;
        _controlsParent.gameObject.SetActive(true);
    }
}