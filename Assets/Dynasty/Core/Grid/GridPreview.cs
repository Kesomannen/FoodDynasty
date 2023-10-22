using System;
using UnityEngine;

namespace Dynasty.Core.Grid {

[RequireComponent(typeof(Renderer))]
public class GridPreview : MonoBehaviour {
    Renderer _renderer;
    
    static readonly int _mousePosition = Shader.PropertyToID("_MousePosition");

    void Awake() {
        _renderer = GetComponent<Renderer>();
    }

    void Update() {
        _renderer.sharedMaterial.SetVector(_mousePosition, Input.mousePosition);
    }
}

}