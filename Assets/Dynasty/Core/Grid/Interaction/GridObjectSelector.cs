using Dynasty.Core.Tooltip;
using Dynasty.Library;
using Dynasty.Library.Events;
using Dynasty.Library.Extensions;
using UnityEngine;

namespace Dynasty.Core.Grid {

/// <summary>
/// Applies an outline to <see cref="GridObject"/>s and shows a tooltip.
/// </summary>
public class GridObjectSelector : MonoBehaviour {
    [Tooltip("When raised, selects the given grid object.")]
    [SerializeField] GameEvent<GridObject> _selectObjectEvent;
    
    [Tooltip("When any are raised, deselects the given grid object.")]
    [SerializeField] GameEvent<GridObject>[] _deselectObjectEvents;
    
    [Tooltip("Shown when selecting a grid object.")]
    [SerializeField] TooltipData<Entity> _tooltipData;

    GridObject _selectedObject;
    GridOutline _selectedOutline;
    
    void OnEnable() {
        _selectObjectEvent.AddListener(OnObjectSelected);
        foreach (var deselectEvent in _deselectObjectEvents) {
            deselectEvent.AddListener(OnObjectDeselected);
        }
    }
    
    void OnDisable() {
        _selectObjectEvent.RemoveListener(OnObjectSelected);
        foreach (var deselectEvent in _deselectObjectEvents) {
            deselectEvent.RemoveListener(OnObjectDeselected);
        }
    }

    void OnObjectSelected(GridObject obj) {
        ChangeSelection(obj);
    }
    
    void OnObjectDeselected(GridObject obj) {
        if (_selectedObject != obj) return;
        ChangeSelection(null);
    }

    void ChangeSelection(GridObject newSelection) {
        if (_selectedObject != null) {
           _selectedObject.GetComponentsInChildren<IOnDeselectedHandler>()
                .ForEach(h => h.OnDeselected());
            
            _selectedOutline.Remove(Color.white);
            _selectedOutline = null;
            _tooltipData.Hide();
        }

        _selectedObject = newSelection; 
        if (_selectedObject == null) return;

        _selectedObject.GetComponentsInChildren<IOnSelectedHandler>()
            .ForEach(h => h.OnSelected());

        _selectedOutline = _selectedObject.GetComponent<GridOutline>();
        _selectedOutline.Require(Color.white);

        if (_selectedObject.TryGetComponent(out Entity entity)) {
            _tooltipData.Show(entity);
        }
    }
}

public interface IOnSelectedHandler {
    void OnSelected();
}

public interface IOnDeselectedHandler {
    void OnDeselected();
}

public interface ISelectionHandler : IOnSelectedHandler, IOnDeselectedHandler { }

}