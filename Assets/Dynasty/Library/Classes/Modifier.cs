using System;

namespace Dynasty.Library {

[Serializable]
public struct Modifier {
    public double Base;
    public double Percentual;
    public double Additive;
    public double Multiplicative;

    public Modifier(double @base = 0, double percentual = 0, double additive = 0, double multiplicative = 1) {
        Base = @base;
        Percentual = percentual;
        Additive = additive;
        Multiplicative = multiplicative;
    }

    public double Delta => Apply(0);
    public float DeltaFloat => Apply(0);

    public double Apply(double value) {
        return (value + Base) * (1 + Percentual) * Multiplicative + Additive;
    }
    
    public float Apply(float value) {
        return (float)Apply((double)value);
    }

    public int Apply(int value) {
        return (int)Apply((double)value);
    }
    
    public static Modifier Lerp(Modifier a, Modifier b, double t) {
        return new Modifier(
            a.Base + (b.Base - a.Base) * t,
            a.Percentual + (b.Percentual - a.Percentual) * t,
            a.Additive + (b.Additive - a.Additive) * t,
            a.Multiplicative + (b.Multiplicative - a.Multiplicative) * t
        );
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
    
    public override string ToString() {
        var result = "";
        if (Base != 0) {
            result += $"{Base:0.#} ";
        }
        if (Percentual != 0) {
            result += $"+{Percentual:P0} ";
        }
        if (Additive != 0) {
            result += $"+{Additive:0.#} ";
        }
        if (Multiplicative != 1) {
            result += $"x{Multiplicative:0.#} ";
        }
        
        return result == "" ? "x1" : result.Trim();
    }
}

}