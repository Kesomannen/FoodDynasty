using System.Collections;
using Dynasty.Library;
using UnityEngine;

namespace Dynasty.UI.Misc {

[RequireComponent(typeof(RectTransform))]
public class WorldCanvasWindow : MonoBehaviour {
    [SerializeField] GenericGameEvent _closeEvent;
    
    RectTransform _rectTransform;

    void OnEnable() {
        _closeEvent.AddListener(Close);
    }
    
    void OnDisable() {
        _closeEvent.RemoveListener(Close);
    }

    void Close() {
        gameObject.SetActive(false);
    }

    public void Show(Vector3 worldPosition) {
        _rectTransform ??= GetComponent<RectTransform>();
        gameObject.SetActive(true);
        
        var worldCanvas = WorldCanvas.Instance;
        var canvas = worldCanvas.Canvas;
        var cam = worldCanvas.Camera;
        
        StartCoroutine(ShowCoroutine());
        return;

        IEnumerator ShowCoroutine() {
            while (enabled) {
                var screenPosition = cam.WorldToScreenPoint(worldPosition);
                RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, screenPosition, cam, out var localPoint);
                _rectTransform.localPosition = localPoint;
                
                yield return null;
            }
        }
    }
}

}