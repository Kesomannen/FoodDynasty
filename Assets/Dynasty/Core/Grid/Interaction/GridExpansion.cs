using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dynasty.Core.Grid {

public class GridExpansion : MonoBehaviour, IPointerClickHandler {
    [SerializeField] Renderer _renderer;
    [SerializeField] Material _activeMaterial;
    [Layer]
    [SerializeField] int _activeLayer;

    public Vector2Int Position { get; private set; }
    public bool IsActive { get; private set; }
    
    public event Action<GridExpansion> OnClicked;
    
    public void Initialize(Vector2Int position) {
        Position = position;
    }
    
    public void Activate() {
        IsActive = true;
        _renderer.material = _activeMaterial;
        _renderer.gameObject.layer = _activeLayer;
    }
    
    public void OnPointerClick(PointerEventData eventData) {
        if (IsActive) return;
        OnClicked?.Invoke(this);
    }
}

}