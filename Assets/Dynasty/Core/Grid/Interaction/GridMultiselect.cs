using System.Collections.Generic;
using System.Linq;
using Dynasty.Library;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Dynasty.Grid {

public class GridMultiselect : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    [SerializeField] float _raycastDistance;
    [SerializeField] float _triggerDuration;
    [SerializeField] MeshCollider _collider;
    [SerializeField] PhysicsEvents _physicsEvents;
    [FormerlySerializedAs("_placer")] 
    [SerializeField] GridObjectMover _mover;
    [SerializeField] Image _selectionImage;
    
    Vector2 _startPosition;
    bool _isSelecting;
    
    RectTransform SelectionRect => _selectionImage.rectTransform;
    
    Mesh _mesh;
    Vector3 _size;
    Vector3 _center;

    void Awake() {
        _mesh = new Mesh();
        _mesh.MarkDynamic();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        _isSelecting = true;
        
        _selectionImage.gameObject.SetActive(true);

        SelectionRect.position = eventData.position;
        _startPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData) {
        if (!_isSelecting) return;
        
        var delta = eventData.position - _startPosition;
        SelectionRect.pivot = new Vector2(delta.x < 0 ? 1 : 0, delta.y < 0 ? 1 : 0);
        SelectionRect.sizeDelta = new Vector2(Mathf.Abs(delta.x), Mathf.Abs(delta.y));
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (!_isSelecting) return;
        _isSelecting = false;
        
        _selectionImage.gameObject.SetActive(false);
        
        var cam = Camera.main;
        var camTransform = cam!.transform;
        
        var a = _startPosition;
        var b = eventData.position;
        var c = new Vector2(a.x, b.y);
        var d = new Vector2(b.x, a.y);

        var aHit = Raycast(a);
        var bHit = Raycast(b);
        var cHit = Raycast(c);
        var dHit = Raycast(d);
        
        Debug.DrawLine(aHit, cHit, Color.green);
        Debug.DrawLine(cHit, bHit, Color.green);
        Debug.DrawLine(bHit, dHit, Color.green);
        Debug.DrawLine(dHit, aHit, Color.green);
        
        _mesh.Clear();
        _mesh.vertices = new[] {
            aHit, cHit, bHit, dHit, camTransform.position
        };
        _mesh.triangles = new[] {
            0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 1, 0, 2, 4, 0, 3, 1
        };
        _mesh.RecalculateNormals();

        var selected = new HashSet<GridObject>();
        _physicsEvents.OnTriggerEnterEvent += TriggerEnter;

        _collider.sharedMesh = _mesh;
        _collider.enabled = true;

        LeanTween.delayedCall(_triggerDuration, () => {
            _physicsEvents.OnTriggerEnterEvent -= TriggerEnter;

            _collider.sharedMesh = null;
            _collider.enabled = false;

            _mover.StartMoving(selected);
        });
        return;

        Vector3 Raycast(Vector2 screenPos) {
            var ray = cam.ScreenPointToRay(screenPos);
            Debug.DrawRay(ray.origin, ray.direction * _raycastDistance, Color.red);
            return ray.GetPoint(_raycastDistance);
        }
        
        void TriggerEnter(Collider other) {
            var gridObject = other.GetComponentInParent<GridObject>();
            if (gridObject == null) return;
            selected.Add(gridObject);
        }
    }
}

}