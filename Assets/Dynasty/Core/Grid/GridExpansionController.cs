using Dynasty.Library;
using UnityEngine;
using UnityEngine.Events;

namespace Dynasty.Grid {

public class GridExpansionController : MonoBehaviour {
    [SerializeField] GridManager _gridManager;
    [SerializeField] GridExpansionManager _expansionManager;
    [SerializeField] GameObject _floor;
    [SerializeField] GameObject _preview;
    [Space]
    [SerializeField] float _previewTweenDuration;
    [SerializeField] LeanTweenType _previewTweenType;
    [Space]
    [SerializeField] float _animationDuration;
    [SerializeField] LeanTweenType _animationTweenType;
    [SerializeField] ParticleSystem _expansionParticles;
    [SerializeField] GameEvent<CameraShake> _cameraShakeEvent;
    [SerializeField] CameraShake _expandShake;
    [SerializeField] UnityEvent _onExpand;

    void OnEnable() {
        _expansionManager.OnExpansionChanged += SetSize;
        SetSize(_expansionManager.CurrentSize, false);
    }
    
    void OnDisable() {
        _expansionManager.OnExpansionChanged -= SetSize;
    }
    
    public void SetSize(Vector2Int size, bool animate) { 
        _gridManager.SetSize(size);

        if (animate) {
            LeanTween.cancel(_floor);
            var tween = LeanTween.scale(_floor, GetFloorScale(size), _animationDuration).setEase(_animationTweenType);
            
            if (_expansionParticles != null) {
                var shape = _expansionParticles.shape;
                _expansionParticles.Play();
                tween.setOnUpdate((Vector3 v) => shape.scale = v)
                    .setOnComplete(() => _expansionParticles.Stop());
            }
            
            if (_cameraShakeEvent != null) {
                _cameraShakeEvent.Raise(_expandShake);
            }
            
            _onExpand.Invoke();
        } else {
            _floor.transform.localScale = GetFloorScale(size);
        }
    }

    public void StartPreviewing() {
        if (!_expansionManager.TryGetNext(out var next)) return;
        
        LeanTween.cancel(_preview);
        LeanTween.scale(_preview, GetFloorScale(next.Size), _previewTweenDuration)
            .setEase(_previewTweenType);
    }

    public void StopPreviewing() {
        LeanTween.cancel(_preview);
        LeanTween.scale(_preview, _floor.transform.localScale, _previewTweenDuration)
            .setEase(_previewTweenType);
    }

    static Vector3 GetFloorScale(Vector2Int gridSize) {
        return new Vector3(gridSize.x / 4f, 1, gridSize.y / 4f);
    }
}

}