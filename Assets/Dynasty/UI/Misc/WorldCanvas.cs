using UnityEngine;

namespace Dynasty.UI.Misc {

[RequireComponent(typeof(Canvas))]
public class WorldCanvas : MonoBehaviour {
    float _startDistance;
    Vector3 _startScale;
    int _registered;
    
    public static WorldCanvas Instance { get; private set; }
    
    public Canvas Canvas { get; private set; }
    public Camera Camera { get; private set; }
    
    void Awake() {
        Instance = this;
        
        Canvas = GetComponent<Canvas>();
        Camera = Canvas.worldCamera;
        
        _startDistance = Vector3.Distance(Camera.transform.position, transform.position);
        _startScale = transform.localScale;
        
        gameObject.SetActive(false);
    }

    void Update() {
        var distance = Vector3.Distance(Camera.transform.position, transform.position);
        var scale = _startScale * (distance / _startDistance);
        transform.localScale = scale;
    }
    
    public void RegisterElement() {
        _registered++;
        if (_registered == 1) {
            gameObject.SetActive(true);
        }
    }
    
    public void UnregisterElement() {
        _registered--;
        if (_registered == 0) {
            gameObject.SetActive(false);
        }
    }
}

}