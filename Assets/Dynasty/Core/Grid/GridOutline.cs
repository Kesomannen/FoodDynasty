using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dynasty.Core.Grid {

[RequireComponent(typeof(Outline))]
public class GridOutline : MonoBehaviour {
    Outline _outline;
    
    readonly List<Color> _colors = new();
    
    void Awake() {
        _outline = GetComponent<Outline>();
        UpdateState();
    }

    void UpdateState() {
        _outline.enabled = _colors.Count > 0;
        _outline.OutlineColor = _colors.Count > 0 ? _colors[^1] : Color.white;
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
        var index = _colors.IndexOf(color);
        if (index == -1) {
            Push(color);
        } else if (index != _colors.Count - 1) {
            _colors.RemoveAt(index);
            Push(color);
        }
    }
    
    public void Require(Color color, bool require) {
        if (require) Require(color);
        else Remove(color);
    }
}

}