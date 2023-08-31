using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace Dynasty.UI.Menu {

[RequireComponent(typeof(Graphic))]
public class PulsingGraphic : MonoBehaviour {
    [MinMaxSlider(0, 1)]
    [SerializeField] Vector2 _alphaRange;
    [SerializeField] float _alphaSpeed;
    
    Graphic _graphic;

    void Awake() {
        _graphic = GetComponent<Graphic>();
    }

    IEnumerator Start() {
        while (enabled) {
            var alpha = Mathf.Lerp(_alphaRange.x, _alphaRange.y, Mathf.PingPong(Time.time * _alphaSpeed, 1));
            _graphic.color = new Color(_graphic.color.r, _graphic.color.g, _graphic.color.b, alpha);
            yield return null;
        }
    }
}

}