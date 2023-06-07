using System;
using UnityEngine;

[Serializable]
public struct Modifier {
    public float Base;
    public float Percentual;
    public float Additive;
    public float Multiplicative;

    public Modifier(float @base = 0, float percentual = 0, float additive = 0, float multiplicative = 1) {
        Base = @base;
        Percentual = percentual;
        Additive = additive;
        Multiplicative = multiplicative;
    }
    
    public float Apply(float value) {
        return (value + Base) * (1 + Percentual) * Multiplicative + Additive;
    }

    public int Apply(int value) {
        return Mathf.RoundToInt(Apply((float) value));
    }

    public void Bake() {
        Base = Apply(1);
        Percentual = 0;
        Additive = 0;
        Multiplicative = 1;
    }

    public void Modify(float value, ModifierType type, bool bake = false) {
        switch (type) {
            case ModifierType.Base: Base += value; break;
            case ModifierType.Percentual: Percentual += value; break;
            case ModifierType.Additive: Additive += value; break;
            case ModifierType.Multiplicative:
                if (value < 0) {
                    Multiplicative /= Mathf.Abs(value);
                } else {
                    Multiplicative *= value;
                }
                break;
            default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        if (bake) {
            Bake();
        }
    }
    
    public static Modifier operator +(Modifier a, Modifier b) {
        return new Modifier(
            a.Base + b.Base,
            a.Percentual + b.Percentual,
            a.Additive + b.Additive,
            a.Multiplicative * b.Multiplicative
        );
    }
    
    public static Modifier operator -(Modifier a, Modifier b) {
        return new Modifier(
            a.Base - b.Base,
            a.Percentual - b.Percentual,
            a.Additive - b.Additive,
            a.Multiplicative / b.Multiplicative
        );
    }
}

public enum ModifierType {
    Base,
    Percentual,
    Additive,
    Multiplicative
}