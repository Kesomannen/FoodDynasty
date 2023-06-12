using UnityEngine;

public class GridObjectSelector : MonoBehaviour {
    [SerializeField] GameEvent<GridObject> _selectObjectEvent;
    [SerializeField] GameEvent<GridObject> _deselectObjectEvent;
    [Space]
    [SerializeField] GameEvent<TooltipParams> _showTooltipEvent;
    [SerializeField] GenericGameEvent _hideTooltipEvent;

    GridObject _selectedObject;
    Outline _selectedOutline;
    
    void OnEnable() {
        _selectObjectEvent.OnRaised += OnObjectSelected;
        _deselectObjectEvent.OnRaised += OnObjectDeselected;
    }
    
    void OnDisable() {
        _selectObjectEvent.OnRaised -= OnObjectSelected;
        _deselectObjectEvent.OnRaised -= OnObjectDeselected;
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
            _selectedOutline.enabled = false;
            _selectedOutline = null;
            _hideTooltipEvent.Raise();
        }
        
        _selectedObject = newSelection;
        if (_selectedObject == null) return;

        _selectedOutline = _selectedObject.GetOrAddComponent<Outline>();
        _selectedOutline.enabled = true;
        
        if (!_selectedObject.TryGetComponent(out IItemDataProvider dataProvider)) return;
        _showTooltipEvent.Raise(new TooltipParams { Content = dataProvider.Data });
    }
}