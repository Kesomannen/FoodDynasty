using System;
using UnityEngine;

[Serializable]
public struct SerializableVector2Int {
    public int X;
    public int Y;
    
    public SerializableVector2Int(Vector2Int vector) {
        X = vector.x;
        Y = vector.y;
    }
    
    public Vector2Int ToVector2Int() => new(X, Y);

    public override string ToString() {
        return $"({X}, {Y})";
    }
}