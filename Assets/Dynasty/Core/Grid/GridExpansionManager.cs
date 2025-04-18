﻿using System;
using System.Collections.Generic;
using Dynasty.Library;
using UnityEngine;

namespace Dynasty.Grid {

[CreateAssetMenu(menuName = "Manager/Expansion Manager")]
public class GridExpansionManager : MonoScriptable {
    [SerializeField] Vector2Int _startingSize;
    [SerializeField] GridExpansion[] _expansions;

    public int ExpansionIndex { get; private set; } = -1;
    public IReadOnlyList<GridExpansion> Expansions => _expansions;

    public Vector2Int CurrentSize => GetSize(ExpansionIndex);
    
    public event Action<Vector2Int, bool> OnExpansionChanged;

    public override void OnAwake() {
        ExpansionIndex = -1;
    }

    public void Expand(bool animate) {
        if (ExpansionIndex == _expansions.Length - 1) return;
        
        ExpansionIndex++;
        OnExpansionChanged?.Invoke(CurrentSize, animate);
    }
    
    public bool TryGetNext(out GridExpansion expansion) {
        if (ExpansionIndex == _expansions.Length - 1) {
            expansion = default;
            return false;
        }

        expansion = _expansions[ExpansionIndex + 1];
        return true;
    }

    public Vector2Int GetSize(int index) {
        return index == -1 ? _startingSize : _expansions[index].Size;
    }
}

[Serializable]
public class GridExpansion {
    public double Cost;
    public Vector2Int Size;
}

}