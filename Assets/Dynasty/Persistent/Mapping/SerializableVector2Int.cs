using System;
using UnityEngine;

namespace Dynasty.Persistent.Mapping {

[Serializable]
public struct SerializableVector2Int {
    public int X;
    public int Y;
    
    public SerializableVector2Int(Vector2Int vector) {
        X = vector.x;
        Y = vector.y;
    }

    public override string ToString() {
        return $"({X}, {Y})";
    }
    
    public static implicit operator Vector2Int(SerializableVector2Int vector) {
        return new Vector2Int(vector.X, vector.Y);
    }
}

}