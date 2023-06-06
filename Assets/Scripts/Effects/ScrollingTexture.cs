using UnityEngine;

public class ScrollingTexture : MonoBehaviour {
    [SerializeField] Renderer _renderer;
    [SerializeField] Vector2 _scrollSpeed;
    
    void Update() {
        var material = _renderer.material;
        
        var offset = material.mainTextureOffset;
        offset += _scrollSpeed * Time.deltaTime;
        material.mainTextureOffset = offset;
    }
}