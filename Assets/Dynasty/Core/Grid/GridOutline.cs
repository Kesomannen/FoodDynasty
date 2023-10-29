using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dynasty.Grid {

[RequireComponent(typeof(Outline))]
public class GridOutline : MonoBehaviour {
    Outline _outline;
    
    readonly List<Color> _colors = new();
    
    void Awake() {
        _outline = GetComponent<Outline>();
        UpdateState();
    }

    void UpdateState() {
        var enable = _colors.Count > 0;
        
        _outline.enabled = enable;
        if (enable && _outline.OutlineColor != _colors[^1]) {
            _outline.OutlineColor = _colors[^1];
        }
    }
    
    void Push(Color color) {
        _colors.Add(color);
        UpdateState();
    }
    
    public void Remove(Color color) {
        _colors.Remove(color);
        UpdateState();
    }
    
    public void Require(Color color) {
        if (_colors.IndexOf(color) == -1) {
            Push(color);
        }
    }
    
    public void Require(Color color, bool require) {
        if (require) Require(color);
        else Remove(color);
    }
}

}