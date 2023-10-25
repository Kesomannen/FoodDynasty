using System.Collections.Generic;
using System.Linq;
using Dynasty.Core.Grid;
using Dynasty.Library;
using UnityEngine;

namespace Dynasty.Machines {

[RequireComponent(typeof(GridObject))]
public abstract class MachineModifier<T> : MonoBehaviour, IMachineComponent, IInfoProvider, ISelectionHandler, IRangeProvider {
    [SerializeField] float _range;
    [SerializeField] Color _outlineColor = Color.cyan;

    public float Range {
        get => _range;
        set => _range = value;
    }
    
    public Color OutlineColor {
        get => _outlineColor;
        set => _outlineColor = value;
    }

    GridObject _gridObject;
    GridManager GridManager => _gridObject.GridManager;
    
    readonly List<(T Component, GridObject GridObject)> _affected = new();

    public IReadOnlyList<(T Component, GridObject GridObject)> Affected => _affected;

    protected virtual void Awake() {
        _gridObject = GetComponent<GridObject>();

        _gridObject.OnAdded += () => {
            GridManager.OnObjectAdded += OnObjectAdded;
            GridManager.OnObjectRemoved += OnObjectRemoved;
            
            RefreshAffectedObjects();
        };
        
        _gridObject.OnRemoved += () => {
            GridManager.OnObjectAdded -= OnObjectAdded;
            GridManager.OnObjectRemoved -= OnObjectRemoved;
            
            ClearAffectedObjects();
        };
    }
    
    protected virtual void OnDisable() {
        GridManager.OnObjectAdded -= OnObjectAdded;
        GridManager.OnObjectRemoved -= OnObjectRemoved;
    }

    void OnObjectAdded(GridObject obj, Vector2Int position, GridRotation rotation) {
        if (obj != _gridObject && InRange(obj)) {
            TryAdd(obj);
        }
    }

    void OnObjectRemoved(GridObject obj) {
        if (_affected.Any(t => t.GridObject == obj)) {
            Remove(obj);
        }
    }

    void RefreshAffectedObjects() {
        ClearAffectedObjects();
        GridManager.GridObjects.Where(InRange).ForEach(TryAdd);
    }

    void ClearAffectedObjects() {
        foreach (var (component, _) in _affected) {
            OnRemoved(component);
        }
        _affected.Clear();
    }

    void TryAdd(GridObject obj) {
        obj.GetComponentsInChildren<T>()
            .Where(Predicate)
            .ForEach(component => {
                _affected.Add((component, obj));
                OnAdded(component);
            });
    }
    
    void Remove(GridObject obj) {
        _affected
            .Where(tuple => tuple.GridObject == obj)
            .ToArray()
            .ForEach(match => {
                _affected.Remove(match);
                OnRemoved(match.Component);
            });
    }
    
    bool InRange(GridObject obj) {
        return Vector2.Distance(_gridObject.GridPosition, obj.GridPosition) <= _range;
    }
    
    protected virtual void OnRemoved(T component) { }
    protected virtual void OnAdded(T component) { }
    protected virtual bool Predicate(T component) => true;

    public void OnSelected() {
        foreach (var obj in Affected.Select(tuple => tuple.GridObject)) {
            obj.GetComponent<GridOutline>()?.Require(_outlineColor);
        }
    }

    public virtual void OnDeselected() {
        foreach (var obj in Affected.Select(tuple => tuple.GridObject)) {
            obj.GetComponent<GridOutline>()?.Remove(_outlineColor);
        }
    }

    public virtual IEnumerable<EntityInfo> GetInfo() {
        yield return new EntityInfo("Range", $"{_range/4:0.#}");
    }

    public Component Component => this;
}

}